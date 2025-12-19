using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100000)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Range(0, 100000)]
    public int StockQuantity { get; set; }

    // 👇 Cloudinary image upload
    public IFormFile Image { get; set; } = null!;
}
