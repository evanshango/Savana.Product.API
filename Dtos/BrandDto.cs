namespace Savana.Product.API.Dtos; 

public class BrandDto {
    public string? BrandId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? Products { get; set; }
}