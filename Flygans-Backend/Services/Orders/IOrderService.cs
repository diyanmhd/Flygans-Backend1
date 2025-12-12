using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Orders
{
    public interface IOrderService
    {
        // Create a new order (Checkout)
        Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto);

        // Get all orders for a specific user (My Orders)
        Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId);

        // Get a specific order, ensuring the user has permission
        Task<ServiceResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId);
    }
}
