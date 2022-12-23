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

public class BrandService : IBrandService {
    private readonly IUnitOfWork _unitOfWork;

    public BrandService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedList<BrandEntity>> GetBrands(BrandParams bParams) {
        var brandSpec = new BrandSpecification(bParams);
        return await _unitOfWork.Repository<BrandEntity>().GetPagedAsync(brandSpec, bParams.Page, bParams.PageSize);
    }

    public async Task<BrandDto?> AddBrand(BrandReq brandReq, string createdBy) {
        var existing = await FetchBrand(slug: null, name: brandReq.Name);

        if (existing != null) return existing.MapBrandToDto();

        var newBrand = new BrandEntity { Name = brandReq.Name, CreatedBy = createdBy };
        newBrand.Slug = newBrand.GetSlug();

        var res = _unitOfWork.Repository<BrandEntity>().AddAsync(newBrand);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapBrandToDto();
    }

    public async Task<BrandDto?> GetBrandBySlug(string slug) {
        var existing = await FetchBrand(slug: slug, name: null);
        return existing?.MapBrandToDto();
    }

    public async Task<BrandEntity?> GetBrandById(string brandId) => await _unitOfWork
        .Repository<BrandEntity>().GetByIdAsync(brandId);
    
    public async Task<BrandDto?> UpdateBrand(string slug, string updatedBy, BrandReq brandReq) {
        var existing = await FetchBrand(slug: slug, name: null);
        if (existing == null) return null;

        var existingName = await FetchBrand(slug: null, name: brandReq.Name);
        if (existingName != null) return existingName.MapBrandToDto();

        existing.Name = brandReq.Name ?? existing.Name;
        existing.Slug = existing.GetSlug();
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveBrandChanges(existing);
    }

    public async Task<BrandDto?> DeleteBrand(string slug, string updatedBy) {
        var existing = await FetchBrand(slug: slug, name: null);
        if (existing == null) return null;

        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        return await SaveBrandChanges(existing);
    }

    private async Task<BrandEntity?> FetchBrand(string? slug, string? name) {
        var brandSpec = new BrandSpecification(brandSlug: slug, brandName: name);
        return await _unitOfWork.Repository<BrandEntity>().GetEntityWithSpec(brandSpec);
    }

    private async Task<BrandDto?> SaveBrandChanges(BrandEntity existingBrand) {
        var res = _unitOfWork.Repository<BrandEntity>().UpdateAsync(existingBrand);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapBrandToDto();
    }
}