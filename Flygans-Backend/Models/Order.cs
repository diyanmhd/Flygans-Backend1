using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderNumber { get; set; } = string.Empty;

        // FK → User
        public int UserId { get; set; }

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        // Order → OrderItems (1:N)
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Order → Payment(s) (1:N)
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
