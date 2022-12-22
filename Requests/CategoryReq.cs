using System.ComponentModel.DataAnnotations;

namespace Savana.Product.API.Requests;

public class CategoryReq {
    [Required(ErrorMessage = "Category Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Category Icon is required")]
    public string? Icon { get; set; }

    [Required(ErrorMessage = "Category Color is required")]
    public string? Color { get; set; }
}