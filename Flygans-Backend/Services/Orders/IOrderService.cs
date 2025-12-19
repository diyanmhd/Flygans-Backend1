using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;

namespace Flygans_Backend.Services.Orders
{
    public interface IOrderService
    {
        Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto);

        Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId);

        Task<ServiceResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId);

        // ⭐ ADMIN: GET ALL ORDERS
        Task<ServiceResponse<List<OrderResponseDto>>> GetAllOrders();

        // ⭐ NEW ADMIN: DELETE ORDER
        Task<ServiceResponse<bool>> DeleteOrder(int orderId);

        // ⭐ NEW ADMIN: UPDATE ORDER STATUS
        Task<ServiceResponse<bool>> UpdateOrderStatus(int orderId, OrderStatus status);
    }
}
