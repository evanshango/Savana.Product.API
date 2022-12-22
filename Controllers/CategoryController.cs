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

[ApiController, Route("categories"), Produces("application/json"), Tags("Categories")]
public class CategoryController : ControllerBase {
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet("")]
    public async Task<ActionResult<PagedList<CategoryDto>>> GetCategories([FromQuery] CategoryParams categoryParams) {
        var categories = await _categoryService.GetCategories(categoryParams);
        Response.AddPaginationHeader(categories.MetaData);
        return Ok(categories.Select(c => c.MapCategoryToDto()).ToList());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> AddCategory([FromBody] CategoryReq categoryReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();
        var response = await _categoryService.AddCategory(categoryReq, createdBy);
        return response != null
            ? CreatedAtRoute("GetCategory", new { slug = response.Slug }, response)
            : BadRequest(new ApiException(400, "Unable to Create Category"));
    }

    [HttpGet("{slug}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDto>> GetCategory([FromRoute] string slug) {
        var res = await _categoryService.GetCategoryBySlug(slug);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Category with slug '{slug}' not found"));
    }

    [HttpPut("{slug}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>>
        UpdateCategory([FromRoute] string slug, [FromBody] CategoryReq catReq) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _categoryService.UpdateCategory(slug, updatedBy, catReq);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Category with slug '{slug}' not found"));
    }

    [HttpDelete("{slug}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory([FromRoute] string slug) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _categoryService.DeleteCategory(slug, updatedBy);
        return res != null ? NoContent() : NotFound(new ApiException(404, $"Category with slug '{slug}' not found"));
    }
}