using System;
using System.ComponentModel.DataAnnotations;

namespace Flygans_Backend.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // Razorpay tracking — NOT FK
        public string OrderNumber { get; set; } = string.Empty;

        // Foreign key to ORDER
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        public long AmountInPaise { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
