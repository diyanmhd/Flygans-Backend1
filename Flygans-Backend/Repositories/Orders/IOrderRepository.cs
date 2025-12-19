using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);

        Task<List<Order>> GetAllOrders();              // existing admin
        Task<bool> DeleteOrder(int orderId);           // NEW
        Task<bool> UpdateOrderStatus(int orderId, OrderStatus status); // NEW
    }
}
