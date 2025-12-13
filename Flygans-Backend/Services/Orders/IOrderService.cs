using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Orders
{
    public interface IOrderService
    {
        Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto);

        Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId);

        Task<ServiceResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId);
    }
}
