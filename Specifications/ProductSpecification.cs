using Savana.Product.API.Entities;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Product.API.Specifications;

public class ProductSpecification : SpecificationService<ProductEntity> {
    public ProductSpecification(ProductParams prodParams) : base(p =>
        (string.IsNullOrEmpty(prodParams.Name) || p.Name!.ToLower().Contains(prodParams.Name.ToLower())) &&
        (prodParams.Enabled != null ? p.Active == prodParams.Enabled : p.Active == true)
    ) {
        AddInclude(p => p.ProductImages);

        if (string.IsNullOrEmpty(prodParams.OrderBy)) AddOrderByDesc(p => p.CreatedAt);

        switch (prodParams.OrderBy) {
            case "createdAt":
                AddOrderByAsc(p => p.CreatedAt);
                break;
            case "createdAtDesc":
                AddOrderByDesc(p => p.CreatedAt);
                break;
            case "name":
                AddOrderByAsc(p => p.Name!);
                break;
            case "nameDesc":
                AddOrderByDesc(p => p.Name!);
                break;
            default:
                AddOrderByDesc(p => p.CreatedAt);
                break;
        }
    }

    public ProductSpecification(string productId) : base(p => p.Id.Equals(productId)) {
        AddInclude(p => p.Brand!);
        AddInclude($"{nameof(ProductEntity.ProductCategories)}.{nameof(ProductCategory.Category)}");
        AddInclude(p => p.ProductImages);
    }
}