using Flygans_Backend.Models;

namespace Flygans_Backend.DTOs.Orders
{
    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public OrderItemResponseDto(OrderItem item)
        {
            ProductId = item.ProductId;
            ProductName = item.Product.Name;
            UnitPrice = item.UnitPrice;
            Quantity = item.Quantity;
        }

        public OrderItemResponseDto() { }
    }
}
