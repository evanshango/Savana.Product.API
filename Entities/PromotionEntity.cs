using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class PromotionEntity : BaseEntity {
    [Required] public string? Title { get; set; }
    [Required] public string? Description { get; set; }
    public string? BrandId { get; set; }
    public virtual BrandEntity? Brand { get; set; }
    public string? CategoryId { get; set; }
    public virtual CategoryEntity? Category { get; set; }
    public string? ProductId { get; set; }
    public virtual ProductEntity? Product { get; set; }
    public string? PromoType { get; set; }

    [Required, Range(1, 100, ErrorMessage = "Value should be between 1 and 100")]
    public double Discount { get; set; }

    [Required] public DateTime ExpiresAt { get; set; }
}