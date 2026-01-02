using Flygans_Backend.Dtos.Admin;
using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Admin
{
    public interface IAdminDashboardRepository
    {
        // ===== EXISTING (DO NOT CHANGE) =====
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalProductsAsync();
        Task<int> GetTotalOrdersAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);

        // ===== NEW (FOR ADMIN DASHBOARD) =====

        // Revenue graph (last 7 days)
        Task<List<DailyRevenueDto>> GetRevenueLast7DaysAsync();

        // Recent activity
        Task<List<RecentUserDto>> GetRecentUsersAsync(int count);
        Task<List<RecentProductDto>> GetRecentProductsAsync(int count);
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count);
    }
}
