using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Models;
using Flygans_Backend.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

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

        // ================= USER =================

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

        // ================= ADMIN =================

        // ⭐ ADMIN: GET ALL ORDERS
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

        // ⭐ ADMIN: UPDATE ORDER STATUS (NO DTO VERSION)
        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] JsonElement body
        )
        {
            // 1️⃣ Read "status" from JSON body
            if (!body.TryGetProperty("status", out var statusProp))
                return BadRequest("Status is required.");

            var statusString = statusProp.GetString();

            // 2️⃣ Convert string → enum safely
            if (!Enum.TryParse<OrderStatus>(statusString, true, out var parsedStatus))
                return BadRequest("Invalid order status.");

            // 3️⃣ Update order
            var response = await _orderService.UpdateOrderStatus(orderId, parsedStatus);
            return Ok(response);
        }
    }
}
