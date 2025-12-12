using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }

    [Required]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    // 🔥 IMPORTANT: Add this
    public Payment? Payment { get; set; }
}
