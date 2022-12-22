using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class ProductEntity : BaseEntity {
    [Required] public string? Name { get; set; }
    [Required] public string? Description { get; set; }
    [Required] public string? Detail { get; set; }
    [Required] public int Quantity { get; set; }
    [Required] public double Price { get; set; }
    [Required] public string? BrandId { get; set; }
    public virtual BrandEntity? Brand { get; set; }
    [Required] public ICollection<ProductCategory>? ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductImage>? ProductImages { get; set; } = new List<ProductImage>();
    [Required] public string? Owner { get; set; }
}