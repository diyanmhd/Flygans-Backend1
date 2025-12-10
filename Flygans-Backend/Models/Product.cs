using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // ✅ FOREIGN KEY TO CATEGORY
    [Required]
    public int CategoryId { get; set; }

    // ✅ NAVIGATION PROPERTY
    public Category Category { get; set; } = null!;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
}
