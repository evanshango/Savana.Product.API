using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Extensions;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Savana.Product.API.Specifications;
using Treasures.Common.Interfaces;

namespace Savana.Product.API.Services;

public class PromotionService : IPromotionService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IUnitOfWork unitOfWork, ILogger<PromotionService> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public IReadOnlyList<PromotionDto> FetchPromotions(PromotionParams promoParams) {
        var promoSpec = new PromotionSpecification(promoParams);
        var promotions = _unitOfWork
            .Repository<PromotionEntity>().GetRandomAsync(promoSpec, promoParams.Size);
        return promotions.Select(p => p.MapPromotionToDto()).ToList();
    }

    public async Task<PromotionDto?> AddPromo(
        string type, PromotionReq req, BrandEntity? brand, CategoryEntity? cat, ProductEntity? prod, string createdBy
    ) {
        var newPromo = new PromotionEntity {
            Title = req.Title, Description = req.Description, BrandId = brand?.Id, Brand = brand, CategoryId = cat?.Id,
            Category = cat, ProductId = prod?.Id, Product = prod, Discount = req.Discount,
            ExpiresAt = GetExpiryTime(req.ExpiresAfter), CreatedBy = createdBy, PromoType = type
        };

        var res = _unitOfWork.Repository<PromotionEntity>().AddAsync(newPromo);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res.MapPromotionToDto();
        _logger.LogError("Error while creating Brand with name {Name}", req.Title);
        return null;
    }

    public async Task<PromotionEntity?> FetchPromotion(string promoId) => await GetPromotion(promoId, null);

    public async Task<PromotionDto?> UpdatePromotion(
        PromotionEntity existing, BrandEntity? brand, CategoryEntity? category, ProductEntity? product,
        PromotionReq promoReq, string updatedBy
    ) {
        existing.Title = promoReq.Title ?? existing.Title;
        existing.Description = promoReq.Description ?? existing.Description;
        existing.BrandId = brand != null ? brand.Id : existing.BrandId;
        existing.Brand = brand ?? existing.Brand;
        existing.CategoryId = category != null ? category.Id : existing.CategoryId;
        existing.Category = category ?? existing.Category;
        existing.BrandId = product != null ? product.Id : existing.ProductId;
        existing.Product = product ?? existing.Product;
        existing.Discount = promoReq.Discount;
        existing.ExpiresAt = GetExpiryTime(promoReq.ExpiresAfter);
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        var update = await SavePromotionChanges(existing);
        return update?.MapPromotionToDto();
    }

    public async Task<PromotionEntity?> DeletePromotion(PromotionEntity existing, string updatedBy) {
        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SavePromotionChanges(existing);
    }

    public async Task<bool> FetchPromotionByTitle(string? title) => await GetPromotion(null, title) != null;

    private async Task<PromotionEntity?> GetPromotion(string? id, string? title) {
        var promoSpec = new PromotionSpecification(promoId: id, title: title);
        return await _unitOfWork.Repository<PromotionEntity>().GetEntityWithSpec(promoSpec);
    }

    private async Task<PromotionEntity?> SavePromotionChanges(PromotionEntity existingPromotion) {
        var res = _unitOfWork.Repository<PromotionEntity>().UpdateAsync(existingPromotion);
        var result = await _unitOfWork.Complete();

        if (result >= 1) return res;
        _logger.LogError("Error while creating Brand with name {Name}", existingPromotion.Title);
        return null;
    }

    private static DateTime GetExpiryTime(ExpiresAfter? expiresAfter) {
        var expiresAt = DateTime.UtcNow.AddDays(1);
        if (expiresAfter == null) return expiresAt;

        var expType = expiresAfter.Type;
        var duration = expiresAfter.Value;
        if (!string.IsNullOrEmpty(expType)) {
            expiresAt = expType switch {
                "days" => DateTime.UtcNow.AddDays(duration),
                "hours" => DateTime.UtcNow.AddHours(duration),
                _ => DateTime.UtcNow.AddDays(1)
            };
        }

        return expiresAt;
    }
}