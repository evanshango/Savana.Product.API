using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class BrandEntity : BaseEntity {
    [Required] public string? Name { get; set; }
    [Required] public string? Slug { get; set; }
    public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();

    public string? GetSlug() => Name?.Replace(" ", "-").ToLower();
}   