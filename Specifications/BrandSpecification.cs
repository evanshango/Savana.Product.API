using Savana.Product.API.Entities;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Product.API.Specifications;

public class BrandSpecification : SpecificationService<BrandEntity> {
    public BrandSpecification(BrandParams brandParams) : base(b =>
        string.IsNullOrEmpty(brandParams.SearchTerm) || b.Name!.ToLower().Contains(brandParams.SearchTerm.ToLower()) &&
        (brandParams.Enabled == null || b.Active == brandParams.Enabled)
    ) {
        if (brandParams.Products != null) AddInclude(b => b.Products);

        if (string.IsNullOrEmpty(brandParams.OrderBy)) AddOrderByDesc(b => b.CreatedAt);

        switch (brandParams.OrderBy) {
            case "createdAt":
                AddOrderByAsc(b => b.CreatedAt);
                break;
            case "createdAtDesc":
                AddOrderByDesc(b => b.CreatedAt);
                break;
            default:
                AddOrderByDesc(b => b.CreatedAt);
                break;
        }
    }

    public BrandSpecification(string? brandSlug, string? brandName) : base(b =>
        (string.IsNullOrEmpty(brandSlug) || b.Slug!.ToLower().Equals(brandSlug.ToLower())) &&
        (string.IsNullOrEmpty(brandName) || b.Name!.ToLower().Equals(brandName.ToLower()))
    ) {
        AddInclude(b => b.Products);
    }
}