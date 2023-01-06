using System.Text.Json;
using MassTransit;
using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Extensions;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Savana.Product.API.Specifications;
using Treasures.Common.Events;
using Treasures.Common.Helpers;
using Treasures.Common.Interfaces;
using Treasures.Common.Messages;

namespace Savana.Product.API.Services;

public class ProductService : IProductService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;
    private readonly IPublishEndpoint _pub;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IPublishEndpoint publisher) {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _pub = publisher;
    }

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

        if (result >= 1) {
            var product = res.MapProductToDto("single");
            await _pub.Publish(new ProductEvent(res.Id, GenerateJson(product, res), "PRODUCT_CREATED"));
            return product;
        }

        _logger.LogError("Error while creating Brand with name {Name}", productReq.Name);
        return null;
    }

    public async Task<ProductDto?> DeleteProduct(string productId, string updatedBy) {
        var existing = await FetchProduct(productId);
        if (existing == null) {
            _logger.LogWarning("Product with id {Id} not found", productId);
            return null;
        }

        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        var product = await SaveProductChanges(existing);
        await EmitProductEvent(product, existing, "delete");
        return product;
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

        var product = await SaveProductChanges(prod);
        await EmitProductEvent(product, prod, "update");
        return product;
    }

    private async Task<ProductEntity?> FetchProduct(string productId) {
        var prodSpec = new ProductSpecification(productId);
        return await _unitOfWork.Repository<ProductEntity>().GetEntityWithSpec(prodSpec);
    }

    private async Task<ProductDto?> SaveProductChanges(ProductEntity existingProduct) {
        var res = _unitOfWork.Repository<ProductEntity>().UpdateAsync(existingProduct);
        var result = await _unitOfWork.Complete();

        if (result >= 1) return res.MapProductToDto("single");

        _logger.LogError("Error while updating Product with name {Name}", existingProduct.Name);
        return null;
    }

    private async Task EmitProductEvent(ProductDto? productDto, ProductEntity product, string action) {
        if (productDto == null) return;
        var json = GenerateJson(productDto, product);
        switch (action) {
            case "update":
                await _pub.Publish(new ProductEvent(productDto.ProductId!, json, "PRODUCT_UPDATED"));
                break;
            case "delete":
                await _pub.Publish(new ProductEvent(productDto.ProductId!, json, "PRODUCT_DELETED"));
                break;
        }
    }

    private static string GenerateJson(ProductDto productDto, ProductEntity entity) {
        var json = new ProductMessage {
            Id = productDto.ProductId, Name = productDto.Name, InitialPrice = productDto.InitialPrice,
            Active = entity.Active, FinalPrice = productDto.FinalPrice, ImageUrl = productDto.DisplayImage,
            Brand = productDto.Brand, Stock = productDto.InStock, Owner = productDto.Owner,
            UpdatedAt = entity.UpdatedAt, UpdatedBy = entity.UpdatedBy, PromoExpiry = entity.Promotion?.ExpiresAt
        };
        return JsonSerializer.Serialize(json);
    }
}