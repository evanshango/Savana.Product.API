using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;

namespace Savana.Product.API.Interfaces;

public interface IPromotionService {
    IReadOnlyList<PromotionDto> FetchPromotions(PromotionParams promoParams);

    Task<PromotionDto?> AddPromo(
        string type, PromotionReq req, BrandEntity? brand, CategoryEntity? cat, ProductEntity? prod, string createdBy
    );

    Task<PromotionEntity?> FetchPromotion(string promoId);

    Task<PromotionDto?> UpdatePromotion(
        PromotionEntity existing, BrandEntity? brand, CategoryEntity? category, ProductEntity? product,
        PromotionReq promoReq, string updatedBy
    );

    Task<PromotionEntity?> DeletePromotion(PromotionEntity existing, string updatedBy);
    Task<bool> FetchPromotionByTitle(string? promoReqTitle);
}