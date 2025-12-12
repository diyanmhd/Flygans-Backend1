using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // Store OrderNumber instead of OrderId
        public string OrderNumber { get; set; } = string.Empty;

        // FOREIGN KEY (required for proper relationship)
        public int OrderId { get; set; }

        // Navigation
        public Order Order { get; set; }

        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        public long AmountInPaise { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
