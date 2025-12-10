using Flygans_Backend.Services.Wishlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]     // ✅ Only logged-in users can use wishlist
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _service;

    public WishlistController(IWishlistService service)
    {
        _service = service;
    }

    // ✅ ADD TO WISHLIST
    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.AddToWishlist(userId, productId);

        return Ok("Added to wishlist ✅");
    }

    // ✅ REMOVE FROM WISHLIST
    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.RemoveFromWishlist(userId, productId);

        return Ok("Removed from wishlist ✅");
    }

    // ✅ GET USER WISHLIST
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var wishlist = await _service.GetWishlist(userId);

        return Ok(wishlist);
    }
}
