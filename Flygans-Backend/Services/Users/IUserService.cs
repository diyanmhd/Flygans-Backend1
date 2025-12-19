using Flygans_Backend.DTOs.Users;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Users
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserResponseDto>>> GetAllUsersAsync();
        Task<ServiceResponse<bool>> BlockUserAsync(int userId);
        Task<ServiceResponse<bool>> UnblockUserAsync(int userId);
        Task<ServiceResponse<bool>> DeleteUserAsync(int userId);
    }
}
