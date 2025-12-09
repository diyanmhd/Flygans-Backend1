using Flygans_Backend.DTOs;

namespace Flygans_Backend.Services.Auth;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);
    Task<LoginResponseDto> Login(LoginDto dto);

    Task<LoginResponseDto> Refresh(string refreshToken);

    Task Logout(string refreshToken);
}
