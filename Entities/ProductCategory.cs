namespace Savana.Product.API.Entities; 

public class ProductCategory {
    public string? ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public string? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}