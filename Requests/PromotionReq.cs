using System.ComponentModel.DataAnnotations;

namespace Savana.Product.API.Requests;

public class PromotionReq {
    [Required(ErrorMessage = "Title is required")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    public string? BrandId { get; set; }
    public string? CategoryId { get; set; }
    public string? ProductId { get; set; }

    [Required(ErrorMessage = "Discount value is required")]
    public double Discount { get; set; }

    [Required(ErrorMessage = "ExpiresAfter is required")]
    public ExpiresAfter? ExpiresAfter { get; set; }
}

public class ExpiresAfter {
    [Required(ErrorMessage = "Type is required(days, hours)")]
    public string? Type { get; set; }

    [Required(ErrorMessage = "A value is required")]
    public double Value { get; set; }
}