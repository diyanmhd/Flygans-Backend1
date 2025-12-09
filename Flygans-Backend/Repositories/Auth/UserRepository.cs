using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Auth;

public class UserRepository : IUserRepository
{
    private readonly FlyganDbContext _context;

    public UserRepository(FlyganDbContext context)
    {
        _context = context;
    }

    // ✅ Find user by email (LOGIN)
    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    // ✅ Find user by Refresh Token (REFRESH + LOGOUT)
    public async Task<User?> GetByRefreshToken(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
    }

    // ✅ Add new user (REGISTER)
    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }

    // ✅ Save changes
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
