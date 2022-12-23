using Treasures.Common.Helpers;

namespace Savana.Product.API.Requests.Params; 

public class ProductParams: Pagination {
    public string? Name { get; set; }
    public string? OrderBy { get; set; }
    public bool? Enabled { get; set; }
}