namespace Savana.Product.API.Dtos;

public class PromotionDto {
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Product { get; set; }
    public double Discount { get; set; }
    public string? PromoType { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}