using Flygans_Backend.DTOs.Auth;   // ✅ FIXED
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Auth;
using System.Security.Cryptography;
using System.Text;

namespace Flygans_Backend.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUserRepository repo, IJwtTokenService jwt)
    {
        _repo = repo;
        _jwt = jwt;
    }

    // ---------------------
    // REGISTER
    // ---------------------
    public async Task<string> Register(RegisterDto dto)
    {
        var exists = await _repo.GetByEmail(dto.Email);

        if (exists != null)
            throw new Exception("Email already registered");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = Hash(dto.Password),
            RefreshToken = null,
            RefreshTokenExpiryTime = null
        };

        await _repo.Add(user);
        await _repo.Save();

        return "Registration successful";
    }

    // ---------------------
    // LOGIN
    // ---------------------
    public async Task<LoginResponseDto> Login(LoginDto dto)
    {
        var user = await _repo.GetByEmail(dto.Email);

        if (user == null || user.PasswordHash != Hash(dto.Password))
            throw new Exception("Invalid credentials");

        var accessToken = _jwt.CreateToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        // ✅ Save refresh token to DB
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _repo.Save();

        return new LoginResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    // ---------------------
    // REFRESH TOKEN
    // ---------------------
    public async Task<LoginResponseDto> Refresh(string refreshToken)
    {
        var user = await _repo.GetByRefreshToken(refreshToken);

        if (user == null)
            throw new Exception("Invalid refresh token");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new Exception("Refresh token expired");

        var newAccessToken = _jwt.CreateToken(user);

        return new LoginResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = user.RefreshToken!,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    // ---------------------
    // LOGOUT
    // ---------------------
    public async Task Logout(string refreshToken)
    {
        var user = await _repo.GetByRefreshToken(refreshToken);

        if (user == null)
            return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _repo.Save();
    }

    // ---------------------
    // PASSWORD HASHING
    // ---------------------
    private string Hash(string input)
    {
        using var sha = SHA256.Create();

        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}
