using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models
{
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

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        // Cloudinary delete support
        public string PublicId { get; set; } = string.Empty;

        // SOFT DELETE
        public bool IsDeleted { get; set; } = false;

        // ✅ CREATED DATE (for recent products)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
