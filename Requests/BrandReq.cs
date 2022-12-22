using System.ComponentModel.DataAnnotations;

namespace Savana.Product.API.Requests;

public class BrandReq {
    [Required(ErrorMessage = "Brand Name is required")] public string? Name { get; set; }
}