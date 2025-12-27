using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Models;
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

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            int userId = GetUserId();
            var response = await _orderService.CreateOrderAsync(userId, dto);
            return Ok(response);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = GetUserId();
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            int userId = GetUserId();
            var response = await _orderService.GetOrderByIdAsync(orderId, userId);
            return Ok(response);
        }

        // ⭐ ADMIN: GET ALL
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrders();
            return Ok(response);
        }

        // ⭐ ADMIN: DELETE ORDER
        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var response = await _orderService.DeleteOrder(orderId);
            return Ok(response);
        }

        // ⭐ ADMIN: UPDATE STATUS
        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus status)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, status);
            return Ok(response);
        }
    }
}
