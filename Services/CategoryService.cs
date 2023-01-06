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

public class CategoryService : ICategoryService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedList<CategoryEntity>> GetCategories(CategoryParams catParams) {
        var catSpec = new CategorySpecification(catParams);
        return await _unitOfWork.Repository<CategoryEntity>()
            .GetPagedAsync(catSpec, catParams.Page, catParams.PageSize);
    }

    public async Task<CategoryDto?> AddCategory(CategoryReq categoryReq, string createdBy) {
        var existing = await FetchCategory(slug: null, name: categoryReq.Name);
        if (existing != null) {
            _logger.LogWarning("Category with name {Name} already exists", existing.Name);
            return existing.MapCategoryToDto();
        }

        var newCategory = new CategoryEntity {
            Name = categoryReq.Name, Icon = categoryReq.Icon, Color = categoryReq.Color, CreatedBy = createdBy
        };
        newCategory.Slug = newCategory.GetSlug();

        var res = _unitOfWork.Repository<CategoryEntity>().AddAsync(newCategory);
        var result = await _unitOfWork.Complete();

        if (result >= 1) return res.MapCategoryToDto();
        _logger.LogError("Error while creating Category with name {Name}", categoryReq.Name);
        return null;
    }

    public async Task<CategoryDto?> GetCategoryBySlug(string slug) {
        var existing = await FetchCategory(slug: slug, name: null);
        return existing?.MapCategoryToDto();
    }

    public async Task<CategoryEntity?> GetCategoryById(string categoryId) => await _unitOfWork
        .Repository<CategoryEntity>().GetByIdAsync(categoryId);

    public async Task<CategoryDto?> UpdateCategory(string slug, string updatedBy, CategoryReq categoryReq) {
        var existing = await FetchCategory(slug: slug, name: null);
        if (existing == null) {
            _logger.LogWarning("Category with slug {Slug} not found", slug);
            return null;
        }

        var existingName = await FetchCategory(slug: null, name: categoryReq.Name);
        if (existingName != null) {
            _logger.LogWarning("Category with name {Name} already exists", existing.Name);
            return existingName.MapCategoryToDto();
        }

        existing.Name = categoryReq.Name ?? existing.Name;
        existing.Icon = categoryReq.Icon ?? existing.Icon;
        existing.Color = categoryReq.Color ?? existing.Color;
        existing.Slug = existing.GetSlug();
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveCategoryChanges(existing);
    }

    public async Task<CategoryDto?> DeleteCategory(string slug, string updatedBy) {
        var existing = await FetchCategory(slug: slug, name: null);
        if (existing == null) {
            _logger.LogWarning("Category with slug {Slug} not found", slug);
            return null;
        }

        existing.Active = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveCategoryChanges(existing);
    }

    private async Task<CategoryEntity?> FetchCategory(string? slug, string? name) {
        var categorySpec = new CategorySpecification(categorySlug: slug, categoryName: name);
        return await _unitOfWork.Repository<CategoryEntity>().GetEntityWithSpec(categorySpec);
    }

    private async Task<CategoryDto?> SaveCategoryChanges(CategoryEntity existingCategory) {
        var res = _unitOfWork.Repository<CategoryEntity>().UpdateAsync(existingCategory);
        var result = await _unitOfWork.Complete();

        if (result >= 1) return res.MapCategoryToDto();
        _logger.LogError("Error while updating Category with name {Name}", existingCategory.Name);
        return null;
    }
}