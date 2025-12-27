using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flygans_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(
            [FromBody] CreatePaymentRequestDto dto)
        {
            var userId = GetUserId();
            var response = await _paymentService
                .InitiatePaymentAsync(dto, userId);

            return Ok(response);
        }

        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPayment(
            [FromBody] PaymentConfirmationDto dto)
        {
            var response = await _paymentService
                .ConfirmPaymentAsync(dto);

            return Ok(response);
        }

        [HttpGet("by-order-number/{orderNumber}")]
        public async Task<IActionResult> GetPaymentByOrderNumber(
            string orderNumber)
        {
            var response = await _paymentService
                .GetPaymentByOrderNumberAsync(orderNumber);

            return Ok(response);
        }
    }
}
