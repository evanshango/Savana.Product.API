using CloudinaryDotNet.Actions;
using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Requests;
using Savana.Product.API.Specifications;
using Treasures.Common.Interfaces;

namespace Savana.Product.API.Services;

public class FileService : IFileService {
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public FileService(ICloudinaryService cloudinaryService, IUnitOfWork unitOfWork, IConfiguration config) {
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
        _config = config;
    }

    public async Task<FileDto?> UploadMedia(ProductEntity product, MediaReq mediaReq, string createdBy) {
        var productImages = new List<ProductImage>();
        var width = int.Parse(_config["Cloudinary:MediaWidth"] ?? "500");
        var height = int.Parse(_config["Cloudinary:MediaHeight"] ?? "500");

        if (mediaReq.DisplayImage != null) {
            var image = await UploadFile(mediaReq.DisplayImage, width, height, "display", createdBy, product);
            if (image != null) productImages.Add(image);
        }

        if (mediaReq.ShowCaseImages is { Count: > 0 }) {
            foreach (var file in mediaReq.ShowCaseImages) {
                var image = await UploadFile(file, width, height, "showCase", createdBy, product);
                if (image != null) productImages.Add(image);
            }
        }

        product.ProductImages = productImages;
        product.UpdatedBy = createdBy;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ProductEntity>().UpdateAsync(product);
        var result = await _unitOfWork.Complete();
        return result < 1
            ? null
            : new FileDto { Message = "Files uploaded", StatusCode = 201, Detail = "Files uploaded successfully" };
    }

    public async Task<FileDto?> RemoveFile(string url) {
        var fileSpec = new MediaSpecification(url, null);
        var existing = await _unitOfWork.Repository<ProductImage>().GetEntityWithSpec(fileSpec);
        if (existing == null) return null;

        var deletionResult = await DeleteFile(existing.Id);
        if (deletionResult.Result != "ok") return null;
        _unitOfWork.Repository<ProductImage>().DeleteAsync(existing);
        var result = await _unitOfWork.Complete();
        return result < 1
            ? null
            : new FileDto { Message = "File deleted", StatusCode = 200, Detail = "Media file deleted successfully" };
    }

    private async Task<ProductImage?> UploadFile(
        IFormFile file, int w, int h, string tag, string createdBy, ProductEntity prod
    ) {
        var uploadResult = await _cloudinaryService.UploadFile(file, prod.Id, w, h);
        if (uploadResult == null) return null;
        var display = new ProductImage {
            Id = uploadResult.PublicId, ImageUrl = uploadResult.Url.ToString(), Flag = tag, Product = prod,
            ProductId = prod.Id, CreatedBy = createdBy
        };
        var res = _unitOfWork.Repository<ProductImage>().AddAsync(display);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res;
    }

    private async Task<DeletionResult> DeleteFile(string id) => await _cloudinaryService.RemoveFile(id);
}