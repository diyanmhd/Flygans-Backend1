using System.ComponentModel.DataAnnotations;

namespace Flygans_Backend.DTOs.Product;

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

    [Url]
    public string ImageUrl { get; set; } = string.Empty;
}
