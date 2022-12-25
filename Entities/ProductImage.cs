using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class ProductImage: BaseEntity {
    [Required] public string? ImageUrl { get; set; }
    [Required] public string? Flag { get; set; }
    [Required] public string? ProductId { get; set; }
    public virtual ProductEntity? Product { get; set; }

    public ProductImage(string id, string url) {
        Id = id;
        ImageUrl = url;
    }
}