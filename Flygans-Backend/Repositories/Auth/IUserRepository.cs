using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User?> GetByRefreshToken(string refreshToken);
    Task Add(User user);
    Task Save();
}
