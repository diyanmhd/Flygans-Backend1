using Flygans_Backend.DTOs;
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
}
