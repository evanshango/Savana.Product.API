using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Extensions;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Savana.Product.API.Specifications;
using Treasures.Common.Helpers;
using Treasures.Common.Interfaces;

namespace Savana.Product.API.Services;

public class ProductService : IProductService {
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedList<ProductEntity>> GetProducts(ProductParams prodParams) {
        var prodSpec = new ProductSpecification(prodParams);
        return await _unitOfWork.Repository<ProductEntity>()
            .GetPagedAsync(prodSpec, prodParams.Page, prodParams.PageSize);
    }

    public async Task<ProductDto?> GetProductById(string productId) {
        var existing = await FetchProduct(productId);
        return existing?.MapProductToDto("single");
    }

    public async Task<ProductEntity?> GetProduct(string productId) => await FetchProduct(productId);

    public async Task<ProductDto?> AddProduct(
        ProductReq productReq, BrandEntity brand, IEnumerable<CategoryEntity> categories, string createdBy
    ) {
        var newProd = new ProductEntity {
            Name = productReq.Name, Description = productReq.Description, Detail = productReq.Detail,
            Quantity = productReq.Stock, Price = productReq.Price, BrandId = brand.Id, Brand = brand,
            Owner = productReq.Owner, CreatedBy = createdBy
        };

        var prodCategories = categories.Select(category => new ProductCategory
            { Category = category, CategoryId = category.Id, Product = newProd, ProductId = newProd.Id }
        ).ToList();

        newProd.ProductCategories = prodCategories;

        var res = _unitOfWork.Repository<ProductEntity>().AddAsync(newProd);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapProductToDto("single");
    }

    public async Task<ProductDto?> DeleteProduct(string productId, string updatedBy) {
        var existing = await FetchProduct(productId);
        if (existing == null) return null;

        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveProductChanges(existing);
    }

    public async Task<ProductDto?> UpdateProduct(
        ProductReq prodReq, ProductEntity prod, BrandEntity? brand, List<CategoryEntity> categories, string updatedBy
    ) {
        var productCategories = categories.Count > 0
            ? categories.Select(category =>
                new ProductCategory
                    { Category = category, CategoryId = category.Id, Product = prod, ProductId = prod.Id }
            ).ToList()
            : prod.ProductCategories;

        prod.Name = prodReq.Name ?? prod.Name;
        prod.Description = prodReq.Description ?? prod.Description;
        prod.Detail = prodReq.Detail ?? prod.Detail;
        prod.Quantity = prodReq.Stock > 0 ? prodReq.Stock : prod.Quantity;
        prod.Price = prodReq.Price > 0 ? prodReq.Price : prod.Price;
        prod.BrandId = brand != null ? brand.Id : prod.BrandId;
        prod.Brand = brand ?? prod.Brand;
        prod.ProductCategories = productCategories;
        prod.Owner = prodReq.Owner ?? prod.Owner;
        prod.UpdatedBy = updatedBy;
        prod.UpdatedAt = DateTime.UtcNow;
        return await SaveProductChanges(prod);
    }

    private async Task<ProductEntity?> FetchProduct(string productId) {
        var prodSpec = new ProductSpecification(productId);
        return await _unitOfWork.Repository<ProductEntity>().GetEntityWithSpec(prodSpec);
    }

    private async Task<ProductDto?> SaveProductChanges(ProductEntity existingProduct) {
        var res = _unitOfWork.Repository<ProductEntity>().UpdateAsync(existingProduct);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapProductToDto("single");
    }
}