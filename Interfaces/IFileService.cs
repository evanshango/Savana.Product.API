using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;
using Savana.Product.API.Requests;

namespace Savana.Product.API.Interfaces;

public interface IFileService {
    Task<FileDto?> UploadMedia(ProductEntity product, MediaReq mediaReq, string createdBy);
    Task<FileDto?> RemoveFile(string url);
}