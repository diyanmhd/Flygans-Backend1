using Flygans_Backend.Models;

namespace Flygans_Backend.Services.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);

    string GenerateRefreshToken(); // ✅ REQUIRED
}
