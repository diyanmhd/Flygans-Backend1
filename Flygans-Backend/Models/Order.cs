using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flygans_Backend.Models
{
    public class Order
    {
        public int Id { get; set; } // PRIMARY KEY

        public string OrderNumber { get; set; } = string.Empty; // Razorpay tracking

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ UPDATED DEFAULT STATUS
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

        // Navigation → OrderItems
        public List<OrderItem> OrderItems { get; set; } = new();

        // FK → Users.Id
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation → Payments (FK is Payment.OrderId)
        public List<Payment> Payments { get; set; } = new();
    }
}
