namespace Savana.Product.API.Entities; 

public class ProductCategory {
    public string? ProductId { get; set; }
    public virtual ProductEntity? Product { get; set; }
    public string? CategoryId { get; set; }
    public virtual CategoryEntity? Category { get; set; }
}