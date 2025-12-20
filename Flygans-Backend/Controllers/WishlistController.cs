using Flygans_Backend.Services.Wishlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _service;

    public WishlistController(IWishlistService service)
    {
        _service = service;
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var result = await _service.AddToWishlist(userId, productId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var result = await _service.RemoveFromWishlist(userId, productId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var response = await _service.GetWishlist(userId);

        return Ok(response);
    }
}
