using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Interfaces;

public interface IProductService {
    Task<PagedList<ProductEntity>> GetProducts(ProductParams productParams);
    Task<ProductDto?> GetProductById(string productId);

    Task<ProductEntity?> GetProduct(string productId);

    Task<ProductDto?> AddProduct(
        ProductReq productReq, BrandEntity brand, IEnumerable<CategoryEntity> categories, string createdBy
    );

    Task<ProductDto?> DeleteProduct(string productId, string updatedBy);

    Task<ProductDto?> UpdateProduct(
        ProductReq productReq, ProductEntity prod, BrandEntity? brand, List<CategoryEntity> categories, string updatedBy
    );
}