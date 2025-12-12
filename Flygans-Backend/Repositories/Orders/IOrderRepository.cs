using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);

        // NEW METHOD
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
    }
}
