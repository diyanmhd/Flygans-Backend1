using Flygans_Backend.Dtos.Admin;

namespace Flygans_Backend.Services.Admin
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
