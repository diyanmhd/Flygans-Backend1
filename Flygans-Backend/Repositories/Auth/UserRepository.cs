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

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetByRefreshToken(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
