namespace Savana.Product.API.Requests.Params; 

public class PromotionParams {
    public string? Title { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Product { get; set; }
    public int Size { get; set; } = 5;
    public bool? Active { get; set; } = true;
}