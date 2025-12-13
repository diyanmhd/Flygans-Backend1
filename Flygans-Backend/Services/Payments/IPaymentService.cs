using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Payments
{
    public interface IPaymentService
    {
        Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId);

        Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(PaymentConfirmationDto dto);

        Task<ServiceResponse<PaymentResponseDto>> GetPaymentByOrderNumberAsync(string orderNumber);
    }
}
