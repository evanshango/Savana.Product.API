using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class Brand : BaseEntity {
    [Required] public string? Name { get; set; }
    [Required] public string? Slug { get; set; }

    public string? GetSlug() => Name?.Replace(" ", "-").ToLower();
}