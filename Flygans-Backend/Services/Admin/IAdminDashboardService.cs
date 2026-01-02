using Flygans_Backend.Dtos.Admin;

namespace Flygans_Backend.Services.Admin
{
    public interface IAdminDashboardService
    {
        // ===== EXISTING (DO NOT REMOVE) =====
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        // ===== NEW (UNIFIED DASHBOARD) =====
        Task<AdminDashboardResponseDto> GetAdminDashboardAsync();
    }
}
