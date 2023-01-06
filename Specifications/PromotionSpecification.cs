using Savana.Product.API.Entities;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.Product.API.Specifications;

public class PromotionSpecification : SpecificationService<PromotionEntity> {
    public PromotionSpecification(PromotionParams pParams) : base(p =>
        (string.IsNullOrEmpty(pParams.Title) || p.Title!.ToLower().Contains(pParams.Title.ToLower().Trim())) &&
        (string.IsNullOrEmpty(pParams.Brand) || p.Brand!.Name!.ToLower().Equals(pParams.Brand.ToLower().Trim())) &&
        (string.IsNullOrEmpty(pParams.Category) ||
         p.Category!.Name!.ToLower().Equals(pParams.Category.ToLower().Trim())) &&
        (string.IsNullOrEmpty(pParams.Product) ||
         p.Product!.Name!.ToLower().Equals(pParams.Product.ToLower().Trim())) &&
        (pParams.Active == null || p.Active == pParams.Active)
    ) {
        AddInclude(p => p.Brand!);
        AddInclude(p => p.Category!);
        AddInclude(p => p.Product!);
    }

    public PromotionSpecification(string? promoId, string? title) : base(p =>
        (string.IsNullOrEmpty(promoId) || p.Id.Equals(promoId)) &&
        (string.IsNullOrEmpty(title) || p.Title!.ToLower().Equals(title.ToLower().Trim()))
    ) {
        AddInclude(p => p.Brand!);
        AddInclude(p => p.Category!);
        AddInclude(p => p.Product!);
    }
}