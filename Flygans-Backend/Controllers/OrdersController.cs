using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Flygans_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );
        }

        // CREATE ORDER
        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            int userId = GetUserId();
            var response = await _orderService.CreateOrderAsync(userId, dto);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // GET ALL ORDERS FOR LOGGED-IN USER
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = GetUserId();
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(response);
        }

        // GET A SPECIFIC ORDER
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            int userId = GetUserId();
            var response = await _orderService.GetOrderByIdAsync(orderId, userId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
