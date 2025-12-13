using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
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

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(
            [FromBody] CreatePaymentRequestDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Unauthorized user"
                });
            }

            var response = await _paymentService
                .InitiatePaymentAsync(dto, userId.Value);

            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPayment(
            [FromBody] PaymentConfirmationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Invalid payment confirmation data"
                });
            }

            var response = await _paymentService
                .ConfirmPaymentAsync(dto);

            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("by-order-number/{orderNumber}")]
        public async Task<IActionResult> GetPaymentByOrderNumber(
            string orderNumber)
        {
            var response = await _paymentService
                .GetPaymentByOrderNumberAsync(orderNumber);

            return response.Success
                ? Ok(response)
                : NotFound(response);
        }
    }
}
