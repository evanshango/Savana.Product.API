using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;

namespace Savana.Product.API.Extensions;

public static class BrandExtensions {
    public static BrandDto MapBrandToDto(this BrandEntity? brand) {
        return new BrandDto {
            BrandId = brand?.Id, Name = brand?.Name, Slug = brand?.Slug, CreatedAt = brand!.CreatedAt,
            Products = brand?.Products.Count
        };
    }
}