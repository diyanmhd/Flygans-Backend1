namespace Flygans_Backend.Dtos.Admin
{
    public class AdminDashboardResponseDto
    {
        public DashboardStatsDto Stats { get; set; } = new();

        public List<DailyRevenueDto> RevenueLast7Days { get; set; } = new();

        public List<RecentUserDto> RecentUsers { get; set; } = new();

        public List<RecentProductDto> RecentProducts { get; set; } = new();

        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }
}
