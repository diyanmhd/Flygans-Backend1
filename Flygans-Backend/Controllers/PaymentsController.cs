using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
using Flygans_Backend.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        // 1️⃣ INITIATE PAYMENT
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] CreatePaymentRequestDto dto)
        {
            try
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

                var response = await _paymentService.InitiatePaymentAsync(dto, userId.Value);
                return StatusCode(response.Success ? 200 : 400, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // 2️⃣ CONFIRM PAYMENT
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationDto dto)
        {
            try
            {
                var response = await _paymentService.ConfirmPaymentAsync(dto);
                return StatusCode(response.Success ? 200 : 400, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // 3️⃣ GET PAYMENT BY ORDER NUMBER
        [HttpGet("by-order-number/{orderNumber}")]
        public async Task<IActionResult> GetPaymentByOrderNumber(string orderNumber)
        {
            try
            {
                var response = await _paymentService.GetPaymentByOrderNumberAsync(orderNumber);
                return StatusCode(response.Success ? 200 : 404, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }
    }
}
