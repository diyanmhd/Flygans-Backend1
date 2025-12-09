using Flygans_Backend.DTOs.Auth;

namespace Flygans_Backend.Services.Auth;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);

    Task<LoginResponseDto> Login(LoginDto dto);

    // ✅ Refresh using refresh token
    Task<LoginResponseDto> Refresh(string refreshToken);

    // ✅ Logout revoke
    Task Logout(string refreshToken);
}
