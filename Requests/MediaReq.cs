using System.ComponentModel.DataAnnotations;

namespace Savana.Product.API.Requests;

public class MediaReq {
    [Required] public IFormFile? DisplayImage { get; set; }
    [Required] public List<IFormFile>? ShowCaseImages { get; set; }
}