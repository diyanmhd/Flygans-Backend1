namespace Flygans_Backend.Dtos.Admin
{
    public class DashboardStatsDto
    {
        // Summary Counts
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }  // Delivered only revenue

        // Order status Counts (separate values)
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
    }
}
