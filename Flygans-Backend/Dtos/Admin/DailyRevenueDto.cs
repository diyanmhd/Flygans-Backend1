namespace Flygans_Backend.Dtos.Admin
{
    public class DailyRevenueDto
    {
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
        public decimal Revenue { get; set; }
    }
}
