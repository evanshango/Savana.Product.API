using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;

namespace Savana.Product.API.Extensions;

public static class PromotionExtensions {
    public static PromotionDto MapPromotionToDto(this PromotionEntity promotion) {
        return new PromotionDto {
            Id = promotion.Id, Title = promotion.Title, Description = promotion.Description,
            PromoType = promotion.PromoType, Brand = promotion.Brand?.Name, Category = promotion.Category?.Name,
            Product = promotion.Product?.Name, Discount = promotion.Discount, ExpiresAt = promotion.ExpiresAt,
            CreatedAt = promotion.CreatedAt
        };
    }
}