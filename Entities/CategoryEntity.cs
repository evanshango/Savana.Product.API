using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.Product.API.Entities;

public class CategoryEntity : BaseEntity {
    [Required] public string? Name { get; set; }
    [Required] public string? Icon { get; set; }
    [Required] public string? Slug { get; set; }
    [Required] public string? Color { get; set; }

    public string? GetSlug() => Name?.Replace(" ", "-").ToLower();
}