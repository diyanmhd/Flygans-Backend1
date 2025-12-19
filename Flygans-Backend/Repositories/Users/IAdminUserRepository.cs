using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Users
{
    public interface IAdminUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> BlockUserAsync(User user);
        Task<bool> UnblockUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
    }
}
