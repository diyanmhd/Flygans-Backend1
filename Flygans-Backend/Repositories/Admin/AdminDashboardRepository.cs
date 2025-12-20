using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Admin
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly FlyganDbContext _db;

        public AdminDashboardRepository(FlyganDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _db.Users.CountAsync();
        }

        public async Task<int> GetTotalProductsAsync()
        {
            return await _db.Products.CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _db.Orders.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _db.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _db.Orders
                .Where(o => o.Status == status)
                .CountAsync();
        }
    }
}
