using Savana.Product.API.Dtos;
using Savana.Product.API.Entities;

namespace Savana.Product.API.Extensions;

public static class ProductExtensions {
    public static ProductDto MapProductToDto(this ProductEntity? product, string tag) {
        List<string?>? categories = null;
        List<string?>? showCaseImages = null;

        var discount = product?.Promotion?.Discount ?? 0;
        var finalPrice = discount > 0 ? product!.Price * (discount / 100) : product!.Price;

        if (tag.Equals("single")) {
            categories = product?.ProductCategories
                .Select(pC => pC.Category?.Name).ToList();
            showCaseImages = product?.ProductImages
                .Where(pI => !pI.Flag!.Equals("display"))
                .Select(pI => pI.ImageUrl).ToList();
        }

        var display = product?.ProductImages.FirstOrDefault(p => p.Flag!.Equals("display"))?.ImageUrl;

        return new ProductDto {
            ProductId = product?.Id, Name = product?.Name, Description = product?.Description, Detail = product?.Detail,
            InStock = product!.Quantity, InitialPrice = product.Price, Owner = product.Owner, Discount = discount,
            Brand = product.Brand?.Name, Categories = categories, DisplayImage = display, FinalPrice = finalPrice,
            ShowCaseImages = showCaseImages
        };
    }
}