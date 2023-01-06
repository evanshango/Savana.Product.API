using Savana.Product.API.Entities;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Product.API.Specifications;

public class CategorySpecification : SpecificationService<CategoryEntity> {
    public CategorySpecification(CategoryParams catParams) : base(c =>
        (string.IsNullOrEmpty(catParams.Name) || c.Name!.ToLower().Contains(catParams.Name.ToLower())) &&
        (catParams.Enabled == null || c.Active == catParams.Enabled)
    ) {
        if (string.IsNullOrEmpty(catParams.OrderBy)) AddOrderByDesc(c => c.CreatedAt);

        switch (catParams.OrderBy) {
            case "createdAt":
                AddOrderByAsc(c => c.CreatedAt);
                break;
            case "createdAtDesc":
                AddOrderByDesc(c => c.CreatedAt);
                break;
            case "name":
                AddOrderByAsc(c => c.Name!);
                break;
            case "nameDesc":
                AddOrderByDesc(c => c.Name!);
                break;
            default:
                AddOrderByDesc(c => c.CreatedAt);
                break;
        }
    }

    public CategorySpecification(string? categorySlug, string? categoryName) : base(c =>
        (string.IsNullOrEmpty(categorySlug) || c.Slug!.ToLower().Equals(categorySlug.ToLower())) &&
        (string.IsNullOrEmpty(categoryName) || c.Name!.ToLower().Equals(categoryName.ToLower()))
    ) { }
}