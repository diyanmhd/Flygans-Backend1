using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Users
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly FlyganDbContext _context;

        public AdminUserRepository(FlyganDbContext context)
        {
            _context = context;
        }

        // Return only active users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        // Get user even if deleted (for validation)
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> BlockUserAsync(User user)
        {
            if (user.IsDeleted) return false;

            user.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(User user)
        {
            if (user.IsDeleted) return false;

            user.IsBlocked = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            // already deleted → return false
            if (user.IsDeleted)
                return false;

            user.IsDeleted = true;
            user.IsBlocked = true; // optional lock
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
