using System.ComponentModel.DataAnnotations;

namespace Savana.Product.API.Requests;

public class ProductReq {
    [Required(ErrorMessage = "Product name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Product description is required")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Product detail is required")]
    public string? Detail { get; set; }

    [Required(ErrorMessage = "Product stock is required")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Product Categories are required")]
    public IList<string>? Categories { get; set; }

    [Required(ErrorMessage = "Product owner is required")]
    public string? Owner { get; set; }
}   