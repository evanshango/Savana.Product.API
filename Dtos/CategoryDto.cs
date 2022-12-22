namespace Savana.Product.API.Dtos; 

public class CategoryDto {
    public string? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
}