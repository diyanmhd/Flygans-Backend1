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

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var dto = new DashboardStatsDto();

            // TOTAL COUNTS
            dto.TotalUsers = await _repo.GetTotalUsersAsync();
            dto.TotalProducts = await _repo.GetTotalProductsAsync();
            dto.TotalOrders = await _repo.GetTotalOrdersAsync();

            // REVENUE (Delivered only)
            dto.TotalRevenue = await _repo.GetTotalRevenueAsync();

            // ✅ PENDING = PendingPayment + COD
            dto.PendingOrders =
                await _repo.GetOrderCountByStatusAsync(OrderStatus.PendingPayment)
              + await _repo.GetOrderCountByStatusAsync(OrderStatus.COD);

            // OTHER STATUS COUNTS (unchanged meaning)
            dto.ConfirmedOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Confirmed);
            dto.ProcessingOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Processing);
            dto.ShippedOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Shipped);
            dto.DeliveredOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Delivered);
            dto.CancelledOrders = await _repo.GetOrderCountByStatusAsync(OrderStatus.Cancelled);

            return dto;
        }
    }
}
