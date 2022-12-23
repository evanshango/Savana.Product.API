namespace Savana.Product.API.Dtos;

public class ProductDto {
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Detail { get; set; }
    public int InStock { get; set; }
    public double Price { get; set; }
    public string? Brand { get; set; }
    public string? DisplayImage { get; set; }
    public IList<string?>? Categories { get; set; }
    public IList<string?>? ShowCaseImages { get; set; }
    public string? Owner { get; set; }
}