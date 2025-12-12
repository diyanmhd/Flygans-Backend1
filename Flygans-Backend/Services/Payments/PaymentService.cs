using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Payments;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace Flygans_Backend.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;

        public PaymentService(
            IConfiguration config,
            IPaymentRepository paymentRepo,
            IOrderRepository orderRepo)
        {
            _keyId = config["Razorpay:KeyId"];
            _keySecret = config["Razorpay:KeySecret"];
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
        }

        // 1️⃣ INITIATE PAYMENT
        public async Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            var order = await _orderRepo.GetOrderByOrderNumberAsync(dto.OrderNumber);
            if (order == null || order.UserId != userId)
            {
                response.Success = false;
                response.Message = "Order not found or unauthorized.";
                return response;
            }

            long amountInPaise = (long)(dto.Amount * 100);

            try
            {
                RazorpayClient client = new RazorpayClient(_keyId, _keySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", $"order_{dto.OrderNumber}" },
                    { "payment_capture", 1 }
                };

                var razorOrder = client.Order.Create(options);

                var payment = new Flygans_Backend.Models.Payment
                {
                    OrderNumber = dto.OrderNumber,
                    RazorpayOrderId = razorOrder["id"],
                    AmountInPaise = amountInPaise,
                    Status = "Pending"
                };

                await _paymentRepo.CreateAsync(payment);

                response.Data = new PaymentResponseDto
                {
                    OrderNumber = dto.OrderNumber,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    AmountInPaise = amountInPaise,
                    Paid = false,
                    Message = "Payment initiated successfully"
                };

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                response.Data = new PaymentResponseDto
                {
                    Message = ex.InnerException?.Message // ⭐ Return actual SQL error
                };

                return response;
            }
        }

        // 2️⃣ CONFIRM PAYMENT
        public async Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(PaymentConfirmationDto dto)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            try
            {
                string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                if (generatedSignature != dto.RazorpaySignature)
                {
                    response.Success = false;
                    response.Message = "Invalid signature!";
                    return response;
                }

                var payment = await _paymentRepo.GetByOrderNumberAsync(dto.OrderNumber);
                if (payment == null)
                {
                    response.Success = false;
                    response.Message = "Payment record not found.";
                    return response;
                }

                payment.RazorpayPaymentId = dto.RazorpayPaymentId;
                payment.RazorpaySignature = dto.RazorpaySignature;
                payment.Status = "Paid";

                await _paymentRepo.UpdateAsync(payment);

                response.Data = new PaymentResponseDto
                {
                    OrderNumber = dto.OrderNumber,
                    RazorpayOrderId = dto.RazorpayOrderId,
                    RazorpayPaymentId = dto.RazorpayPaymentId,
                    AmountInPaise = payment.AmountInPaise,
                    Paid = true,
                    Message = "Payment verified successfully"
                };

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Signature verification failed: " + ex.Message;

                response.Data = new PaymentResponseDto
                {
                    Message = ex.InnerException?.Message // ⭐ Actual SQL error
                };

                return response;
            }
        }

        // 3️⃣ GET PAYMENT BY ORDER NUMBER
        public async Task<ServiceResponse<PaymentResponseDto>> GetPaymentByOrderNumberAsync(string orderNumber)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            var payment = await _paymentRepo.GetByOrderNumberAsync(orderNumber);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "No payment found for this order.";
                return response;
            }

            response.Data = new PaymentResponseDto
            {
                OrderNumber = orderNumber,
                RazorpayOrderId = payment.RazorpayOrderId,
                RazorpayPaymentId = payment.RazorpayPaymentId,
                AmountInPaise = payment.AmountInPaise,
                Paid = payment.Status == "Paid",
                Message = "Payment details retrieved"
            };

            return response;
        }
    }
}
