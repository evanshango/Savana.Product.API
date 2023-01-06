using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Controllers;

[ApiController, Route("promotions"), Produces("application/json"), Tags("Promotions")]
public class PromotionController : ControllerBase {
    private readonly IPromotionService _promotionService;
    private readonly IBrandService _brandService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public PromotionController(
        IPromotionService promotionService, IBrandService brandService, IProductService productService,
        ICategoryService categoryService
    ) {
        _promotionService = promotionService;
        _brandService = brandService;
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet("")]
    public ActionResult<IReadOnlyList<PromotionDto>> GetPromotions([FromQuery] PromotionParams promoParams) {
        return Ok(_promotionService.FetchPromotions(promoParams));
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<PromotionDto>> AddPromotion(
        [FromQuery] string promoType, [FromBody] PromotionReq promoReq
    ) {
        BrandEntity? brand = null;
        CategoryEntity? category = null;
        ProductEntity? product = null;

        if (!string.IsNullOrEmpty(promoReq.BrandId))
            brand = await _brandService.GetBrandById(promoReq.BrandId);

        if (!string.IsNullOrEmpty(promoReq.CategoryId))
            category = await _categoryService.GetCategoryById(promoReq.CategoryId);

        if (!string.IsNullOrEmpty(promoReq.ProductId))
            product = await _productService.GetProduct(promoReq.ProductId);

        var existingPromo = await _promotionService.FetchPromotionByTitle(promoReq.Title);
        if (existingPromo) return Conflict(new ApiException(409, "Promotion already exists"));

        var promo = await _promotionService.AddPromo(
            GetPromotionType(promoType), promoReq, brand, category, product, User.RetrieveEmailFromPrincipal()
        );
        return promo != null ? Ok(promo) : BadRequest(new ApiException(400, "Unable to add Promotion"));
    }

    [HttpPut("{promoId}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<PromotionDto>> UpdatePromotion(
        [FromRoute] string promoId, [FromBody] PromotionReq promoReq
    ) {
        BrandEntity? brand = null;
        CategoryEntity? category = null;
        ProductEntity? product = null;

        if (!string.IsNullOrEmpty(promoReq.BrandId))
            brand = await _brandService.GetBrandById(promoReq.BrandId);

        if (!string.IsNullOrEmpty(promoReq.CategoryId))
            category = await _categoryService.GetCategoryById(promoReq.CategoryId);

        if (!string.IsNullOrEmpty(promoReq.ProductId))
            product = await _productService.GetProduct(promoReq.ProductId);

        var existing = await _promotionService.FetchPromotion(promoId);
        if (existing == null) return NotFound(new ApiException(404, $"Promotion with id {promoId} not found"));
        var promo = await _promotionService.UpdatePromotion(
            existing, brand, category, product, promoReq, User.RetrieveEmailFromPrincipal()
        );
        return promo != null ? Ok(promo) : BadRequest(new ApiException(400, "Unable to update Promotion"));
    }

    [HttpDelete("{promoId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePromotion([FromRoute] string promoId) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var existing = await _promotionService.FetchPromotion(promoId);
        if (existing == null) return NotFound(new ApiException(404, $"Promotion with id {promoId} not found"));

        var pro = await _promotionService.DeletePromotion(existing, updatedBy);
        return pro != null ? NoContent() : BadRequest(new ApiException(400, "Unable to delete Promotion"));
    }

    private static string GetPromotionType(string promoType) {
        return promoType switch {
            "brand" => "BrandPromo",
            "category" => "CategoryPromo",
            "product" => "ProductPromo",
            _ => "ProductPromo"
        };
    }
}