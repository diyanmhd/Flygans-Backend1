using Flygans_Backend.Dtos.Admin;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Admin;

namespace Flygans_Backend.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repo;

        public AdminDashboardService(IAdminDashboardRepository repo)
        {
            _repo = repo;
        }

        // ================= EXISTING METHOD (UNCHANGED) =================
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var dto = new DashboardStatsDto();

            // TOTAL COUNTS
            dto.TotalUsers = await _repo.GetTotalUsersAsync();
            dto.TotalProducts = await _repo.GetTotalProductsAsync();
            dto.TotalOrders = await _repo.GetTotalOrdersAsync();

            // TOTAL REVENUE (Delivered only)
            dto.TotalRevenue = await _repo.GetTotalRevenueAsync();

            // PENDING = PendingPayment + COD
            dto.PendingOrders =
                await _repo.GetOrderCountByStatusAsync(OrderStatus.PendingPayment)
              + await _repo.GetOrderCountByStatusAsync(OrderStatus.COD);

            // OTHER STATUS COUNTS
            dto.ConfirmedOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Confirmed);
            dto.ProcessingOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Processing);
            dto.ShippedOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Shipped);
            dto.DeliveredOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Delivered);
            dto.CancelledOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Cancelled);

            return dto;
        }

        // ================= NEW METHOD (UNIFIED DASHBOARD) =================
        public async Task<AdminDashboardResponseDto> GetAdminDashboardAsync()
        {
            var response = new AdminDashboardResponseDto
            {
                // Stats (reuse existing method)
                Stats = await GetDashboardStatsAsync(),

                // Revenue graph (last 7 days)
                RevenueLast7Days = await _repo.GetRevenueLast7DaysAsync(),

                // Recent activity
                RecentUsers = await _repo.GetRecentUsersAsync(5),
                RecentProducts = await _repo.GetRecentProductsAsync(5),
                RecentOrders = await _repo.GetRecentOrdersAsync(5)
            };

            return response;
        }
    }
}
