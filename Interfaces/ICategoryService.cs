using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Requests;
using Savana.Product.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Interfaces; 

public interface ICategoryService {
    Task<PagedList<CategoryEntity>> GetCategories(CategoryParams categoryParams);
    Task<CategoryDto?> AddCategory(CategoryReq categoryReq, string createdBy);
    Task<CategoryDto?> GetCategoryBySlug(string slug);
    Task<CategoryEntity?> GetCategoryById(string categoryId);
    Task<CategoryDto?> UpdateCategory(string slug, string updatedBy, CategoryReq categoryReq);
    Task<CategoryDto?> DeleteCategory(string slug, string updatedBy);
}   