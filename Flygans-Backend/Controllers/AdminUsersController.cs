using Flygans_Backend.Services.Users;
using Flygans_Backend.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flygans_Backend.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IAdminDashboardService _dashboard;

        public AdminUsersController(
            IUserService service,
            IAdminDashboardService dashboard)
        {
            _service = service;
            _dashboard = dashboard;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var stats = await _dashboard.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllUsersAsync();
            return Ok(res);
        }

        [HttpPatch("{id}/block")]
        public async Task<IActionResult> Block(int id)
        {
            var res = await _service.BlockUserAsync(id);
            return Ok(res);
        }

        [HttpPatch("{id}/unblock")]
        public async Task<IActionResult> Unblock(int id)
        {
            var res = await _service.UnblockUserAsync(id);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _service.DeleteUserAsync(id);
            return Ok(res);
        }
    }
}
