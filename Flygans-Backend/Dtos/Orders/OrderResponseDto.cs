using Flygans_Backend.Models;

namespace Flygans_Backend.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<OrderItemResponseDto> Items { get; set; } = new();

        public OrderResponseDto(Order order)
        {
            Id = order.Id;
            OrderNumber = order.OrderNumber;
            DeliveryAddress = order.DeliveryAddress;
            PaymentMethod = order.PaymentMethod;
            TotalAmount = order.TotalAmount;
            CreatedAt = order.CreatedAt;
            Status = order.Status.ToString();

            Items = order.OrderItems
                .Select(i => new OrderItemResponseDto(i))
                .ToList();
        }

        public OrderResponseDto() { }
    }
}
