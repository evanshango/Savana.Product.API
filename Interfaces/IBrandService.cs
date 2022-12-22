using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Interfaces; 

public interface IBrandService {
    Task<PagedList<BrandEntity>> GetBrands(BrandParams brandParams);
    Task<BrandDto?> AddBrand(BrandReq brandReq, string createdBy);
    Task<BrandDto?> GetBrandBySlug(string slug);
    Task<BrandDto?> UpdateBrand(string slug, string updatedBy, BrandReq brandReq);
    Task<BrandDto?> DeleteBrand(string slug, string updatedBy);
}   