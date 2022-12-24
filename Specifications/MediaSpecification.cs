using Savana.Product.API.Entities;
using Treasures.Common.Services;

namespace Savana.Product.API.Specifications;

public class MediaSpecification : SpecificationService<ProductImage> {
    public MediaSpecification(string? url, string? flag) : base(pI =>
        (string.IsNullOrEmpty(url) || pI.ImageUrl!.Equals(url)) && (string.IsNullOrEmpty(flag) || pI.Flag!.Equals(flag))
    ) {
        if (flag == null) AddInclude(pI => pI.Product!);
    }
}