using Flygans_Backend.DTOs.Auth;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Auth;
using System.Security.Cryptography;
using System.Text;

namespace Flygans_Backend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtTokenService _jwt;

        public AuthService(IUserRepository repo, IJwtTokenService jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

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
                Role = "User",
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                IsBlocked = false,
                IsDeleted = false
            };

            await _repo.Add(user);
            await _repo.Save();

            return "Registration successful";
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await _repo.GetByEmail(dto.Email);

            if (user == null || user.PasswordHash != Hash(dto.Password))
                throw new Exception("Invalid credentials");

            if (user.IsDeleted)
                throw new Exception("This account was deleted by an admin.");

            if (user.IsBlocked)
                throw new Exception("Your account is blocked. Contact admin.");

            var accessToken = _jwt.CreateToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

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

        public async Task<LoginResponseDto> Refresh(string refreshToken)
        {
            var user = await _repo.GetByRefreshToken(refreshToken);

            if (user == null)
                throw new Exception("Invalid refresh token");

            if (user.IsDeleted)
                throw new Exception("This account was deleted by an admin.");

            if (user.IsBlocked)
                throw new Exception("Your account is blocked. Contact admin.");

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Refresh token expired");

            // 🔥 ROTATE REFRESH TOKEN
            var newRefreshToken = _jwt.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var newAccessToken = _jwt.CreateToken(user);

            await _repo.Save(); // persist rotation

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public async Task Logout(string refreshToken)
        {
            var user = await _repo.GetByRefreshToken(refreshToken);

            if (user == null)
                return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _repo.Save();
        }

        private string Hash(string input)
        {
            using var sha = SHA256.Create();

            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
