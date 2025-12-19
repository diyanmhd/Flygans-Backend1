namespace Flygans_Backend.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
