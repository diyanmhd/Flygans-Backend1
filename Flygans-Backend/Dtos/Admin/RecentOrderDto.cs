using Flygans_Backend.Models;

namespace Flygans_Backend.Dtos.Admin
{
    public class RecentOrderDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
