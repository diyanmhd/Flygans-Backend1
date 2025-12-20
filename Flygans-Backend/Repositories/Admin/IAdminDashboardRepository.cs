using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Admin
{
    public interface IAdminDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalProductsAsync();
        Task<int> GetTotalOrdersAsync();
        Task<decimal> GetTotalRevenueAsync();

        Task<int> GetOrderCountByStatusAsync(OrderStatus status);
    }
}
