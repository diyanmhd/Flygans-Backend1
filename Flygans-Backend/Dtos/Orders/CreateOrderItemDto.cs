namespace Flygans_Backend.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // REQUIRED for TotalAmount calculation
        public decimal Price { get; set; }
    }
}
