using Flygans_Backend.Data;
using Flygans_Backend.Dtos.Admin;
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

        // ================= BASIC STATS =================

        public async Task<int> GetTotalUsersAsync()
        {
            return await _db.Users
                .Where(u => !u.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetTotalProductsAsync()
        {
            return await _db.Products
                .Where(p => !p.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _db.Orders.CountAsync();
        }

        // 💰 TOTAL REVENUE (Delivered + Shipped)
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _db.Orders
                .Where(o =>
                    o.Status == OrderStatus.Delivered ||
                    o.Status == OrderStatus.Shipped
                )
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _db.Orders
                .Where(o => o.Status == status)
                .CountAsync();
        }

        // ================= DASHBOARD DATA =================

        // 📊 Revenue for last 7 days (Delivered + Shipped)
        public async Task<List<DailyRevenueDto>> GetRevenueLast7DaysAsync()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-6);
            var endDate = DateTime.UtcNow.Date;

            var revenueData = await _db.Orders
                .Where(o =>
                    (o.Status == OrderStatus.Delivered ||
                     o.Status == OrderStatus.Shipped) &&
                    o.CreatedAt.Date >= startDate &&
                    o.CreatedAt.Date <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            var result = new List<DailyRevenueDto>();

            // Ensure all 7 days exist (even with zero revenue)
            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                var dayData = revenueData.FirstOrDefault(r => r.Date == date);

                result.Add(new DailyRevenueDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Revenue = dayData?.Revenue ?? 0
                });
            }

            return result;
        }

        // 👤 Recent users
        public async Task<List<RecentUserDto>> GetRecentUsersAsync(int count)
        {
            return await _db.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .Select(u => new RecentUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        // 📦 Recent products
        public async Task<List<RecentProductDto>> GetRecentProductsAsync(int count)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Select(p => new RecentProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        // 🧾 Recent orders
        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count)
        {
            return await _db.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .Select(o => new RecentOrderDto
                {
                    OrderNumber = o.OrderNumber,
                    UserName = o.User.FullName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }
    }
}
