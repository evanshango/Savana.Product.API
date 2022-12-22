using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Product.API.Dtos;
using Savana.Product.API.Extensions;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Controllers;

[ApiController, Route("brands"), Produces("application/json"), Tags("Brands")]
public class BrandController : ControllerBase {
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService) => _brandService = brandService;

    [HttpGet("")]
    public async Task<ActionResult<PagedList<BrandDto>>> GetBrands([FromQuery] BrandParams bParams) {
        var brands = await _brandService.GetBrands(bParams);
        Response.AddPaginationHeader(brands.MetaData);
        return Ok(brands.Select(b => b.MapBrandToDto()).ToList());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> AddBrand([FromBody] BrandReq brandReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();
        var response = await _brandService.AddBrand(brandReq, createdBy);
        return response != null
            ? CreatedAtRoute("GetBrand", new { slug = response.Slug }, response)
            : BadRequest(new ApiException(400, "Unable to Create Brand"));
    }

    [HttpGet("{slug}", Name = "GetBrand")]
    public async Task<ActionResult<BrandDto>> GetBrand([FromRoute] string slug) {
        var res = await _brandService.GetBrandBySlug(slug);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Brand with slug '{slug}' not found"));
    }

    [HttpPut("{slug}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> UpdateBrand([FromRoute] string slug, [FromBody] BrandReq brandReq) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _brandService.UpdateBrand(slug, updatedBy, brandReq);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Brand with slug '{slug}' not found"));
    }

    [HttpDelete("{slug}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrand([FromRoute] string slug) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _brandService.DeleteBrand(slug, updatedBy);
        return res != null ? NoContent() : NotFound(new ApiException(404, $"Brand with slug '{slug}' not found"));
    }
}