using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // FK → Orders.OrderNumber
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey(nameof(OrderNumber))]
        public Order Order { get; set; }

        // Razorpay fields
        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        [Required]
        public long AmountInPaise { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
