using Treasures.Common.Helpers;

namespace Savana.Product.API.Requests.Params; 

public class BrandParams: Pagination {
    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
    public bool? Enabled { get; set; }
    public bool? Products { get; set; }
}