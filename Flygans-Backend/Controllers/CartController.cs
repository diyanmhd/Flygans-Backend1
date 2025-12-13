using Flygans_Backend.Services.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId, [FromQuery] int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.AddToCart(userId, productId, quantity);

        return Ok("Item added to cart");
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateQuantity(int productId, [FromQuery] int quantity)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.UpdateQuantity(userId, productId, quantity);

        return Ok("Quantity updated");
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.RemoveFromCart(userId, productId);

        return Ok("Item removed from cart");
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.ClearCart(userId);

        return Ok("Cart cleared");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var cart = await _service.GetCartItems(userId);

        return Ok(cart);
    }
}
