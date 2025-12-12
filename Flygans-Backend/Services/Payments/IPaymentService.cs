using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Payments
{
    public interface IPaymentService
    {
        // 1️⃣ Create Razorpay order (Initiate Payment)
        Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId);

        // 2️⃣ Verify Razorpay Signature (Confirm Payment)
        Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(PaymentConfirmationDto dto);

        // 3️⃣ Get payment details by OrderNumber (NEW)
        Task<ServiceResponse<PaymentResponseDto>> GetPaymentByOrderNumberAsync(string orderNumber);
    }
}
