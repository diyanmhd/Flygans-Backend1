using Flygans_Backend.DTOs.Auth;  // ✅ FIXED NAMESPACE
using Flygans_Backend.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // ---------------------
    // REGISTER
    // ---------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            return Ok(await _auth.Register(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ---------------------
    // LOGIN
    // ---------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            return Ok(await _auth.Login(dto));
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    // ---------------------
    // REFRESH TOKEN
    // ---------------------
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        try
        {
            return Ok(await _auth.Refresh(dto.RefreshToken));
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    // ---------------------
    // LOGOUT
    // ---------------------
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto dto)
    {
        await _auth.Logout(dto.RefreshToken);
        return Ok("Logged out successfully");
    }
}
