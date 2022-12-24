using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Product.API.Dtos;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Controllers;

[ApiController, Route("media/{productId}"), Produces("application/json"), Tags("Media")]
public class MediaController : ControllerBase {
    private readonly IFileService _fileService;
    private readonly IProductService _productService;

    public MediaController(IFileService fileService, IProductService productService) {
        _fileService = fileService;
        _productService = productService;
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<FileDto>> UploadMedia([FromRoute] string productId, [FromForm] MediaReq mediaReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();
        var existingProduct = await _productService.GetProduct(productId);
        if (existingProduct == null) {
            return NotFound(new ApiException(404, $"Product with id {productId} not found"));
        }

        var res = await _fileService.UploadMedia(existingProduct, mediaReq, createdBy);
        return res != null ? Ok(res) : BadRequest(new ApiException(400, "Unable to upload media"));
    }

    [HttpDelete, Authorize(Roles = "Admin")]
    public async Task<ActionResult<FileDto>> DeleteMedia([FromRoute] string productId, [FromQuery] string url) {
        if (string.IsNullOrEmpty(url)) return BadRequest(new ApiException(400, "File url is required"));

        var product = await _productService.GetProduct(productId);
        if (product == null) {
            return NotFound(new ApiException(404, $"Product with id {productId} not found"));
        }

        var result = await _fileService.RemoveFile(url);
        return result != null ? Ok(result) : NotFound(new ApiException(404, $"File with url {url} not found"));
    }
}