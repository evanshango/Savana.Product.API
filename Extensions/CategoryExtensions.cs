using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;

namespace Savana.Product.API.Extensions;

public static class CategoryExtensions {
    public static CategoryDto MapCategoryToDto(this CategoryEntity? category) {
        return new CategoryDto {
            CategoryId = category?.Id, Name = category?.Name, Icon = category?.Icon, Color = category?.Color,
            Slug = category?.Slug, CreatedAt = category!.CreatedAt
        };
    }
}