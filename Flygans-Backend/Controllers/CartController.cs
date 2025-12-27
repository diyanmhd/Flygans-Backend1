using Flygans_Backend.Services.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flygans_Backend.Controllers
{
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

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> Add(int productId, [FromQuery] int quantity = 1)
        {
            var userId = GetUserId();
            await _service.AddToCart(userId, productId, quantity);

            return Ok(new
            {
                success = true,
                message = "Item added to cart"
            });
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromQuery] int quantity)
        {
            var userId = GetUserId();
            await _service.UpdateQuantity(userId, productId, quantity);

            return Ok(new
            {
                success = true,
                message = quantity <= 0 ? "Item removed from cart" : "Quantity updated"
            });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            await _service.RemoveFromCart(userId, productId);

            return Ok(new
            {
                success = true,
                message = "Item removed from cart"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            await _service.ClearCart(userId);

            return Ok(new
            {
                success = true,
                message = "Cart cleared"
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var cart = await _service.GetCartItems(userId);

            return Ok(new
            {
                success = true,
                message = "Cart retrieved",

                data = cart
            });
        }
    }
}
