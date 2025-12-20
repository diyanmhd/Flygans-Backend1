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
            try
            {
                var userId = GetUserId();
                await _service.AddToCart(userId, productId, quantity);

                return Ok(new
                {
                    success = true,
                    message = "Item added to cart"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromQuery] int quantity)
        {
            try
            {
                var userId = GetUserId();
                await _service.UpdateQuantity(userId, productId, quantity);

                return Ok(new
                {
                    success = true,
                    message = quantity <= 0 ? "Item removed from cart" : "Quantity updated"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                var userId = GetUserId();
                await _service.RemoveFromCart(userId, productId);

                return Ok(new
                {
                    success = true,
                    message = "Item removed from cart"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = GetUserId();
                await _service.ClearCart(userId);

                return Ok(new
                {
                    success = true,
                    message = "Cart cleared"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
