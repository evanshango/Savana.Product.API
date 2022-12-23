using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Extensions;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Controllers;

[ApiController, Route("products"), Produces("application/json"), Tags("Products")]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService prodService, IBrandService brandService, ICategoryService catService) {
        _productService = prodService;
        _brandService = brandService;
        _categoryService = catService;
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedList<ProductDto>>> GetProducts([FromQuery] ProductParams productParams) {
        var products = await _productService.GetProducts(productParams);
        Response.AddPaginationHeader(products.MetaData);
        return Ok(products.Select(p => p.MapProductToDto("multi")).ToList());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductReq productReq) {
        if (productReq.Brand == null)
            return BadRequest(new ApiException(400, "Please provide a product Brand"));
        var brand = await _brandService.GetBrandById(productReq.Brand);
        if (brand == null)
            return NotFound(new ApiException(404, $"Brand with id {productReq.Brand} not found"));

        if (productReq.Categories == null)
            return BadRequest(new ApiException(400, "Please provide at least one product category"));
        var categories = new List<CategoryEntity>();
        foreach (var catId in productReq.Categories) {
            var existingCat = await _categoryService.GetCategoryById(catId);
            if (existingCat == null)
                return NotFound(new ApiException(404, $"Category with id {catId} not found"));
            categories.Add(existingCat);
        }

        var createdBy = User.RetrieveEmailFromPrincipal();
        var response = await _productService.AddProduct(productReq, brand, categories, createdBy);
        return response != null
            ? CreatedAtRoute("GetProduct", new { productId = response.ProductId }, response)
            : BadRequest(new ApiException(400, "Unable to Create Product"));
    }

    [HttpGet("{productId}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDto>> GetProduct([FromRoute] string productId) {
        var res = await _productService.GetProductById(productId);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Product with id '{productId}' not found"));
    }

    [HttpPut("{productId}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        [FromRoute] string productId, [FromBody] ProductReq prodReq
    ) {
        BrandEntity? brand = null;
        if (prodReq.Brand != null) {
            brand = await _brandService.GetBrandById(prodReq.Brand);
        }

        var categories = new List<CategoryEntity>();
        if (prodReq.Categories != null) {
            foreach (var catId in prodReq.Categories) {
                var existingCat = await _categoryService.GetCategoryById(catId);
                if (existingCat != null) categories.Add(existingCat);
            }
        }

        var existing = await _productService.GetProduct(productId);
        if (existing == null) return NotFound(new ApiException(404, $"Product with id '{productId}' not found"));

        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _productService.UpdateProduct(prodReq, existing, brand, categories, updatedBy);
        return res == null ? BadRequest(new ApiException(400, "Unable to Update Product")) : Ok(res);
    }

    [HttpDelete("{productId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string productId) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _productService.DeleteProduct(productId, updatedBy);
        return res != null ? NoContent() : NotFound(new ApiException(404, $"Product with id '{productId}' not found"));
    }
}