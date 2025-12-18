using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Payments;
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

        // ===============================
        // INITIATE PAYMENT (FIXED)
        // ===============================
        public async Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(
            CreatePaymentRequestDto dto, int userId)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            // 1️⃣ Get order from DB
            var order = await _orderRepo.GetOrderByOrderNumberAsync(dto.OrderNumber);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }

            // 2️⃣ Check order ownership
            if (order.UserId != userId)
            {
                response.Success = false;
                response.Message = "Unauthorized order access";
                return response;
            }

            // 3️⃣ Check order status
            if (order.Status == "Paid")
            {
                response.Success = false;
                response.Message = "Order already paid";
                return response;
            }

            // 4️⃣ Calculate amount ONLY from DB
            if (order.TotalAmount <= 0)
            {
                response.Success = false;
                response.Message = "Invalid order amount";
                return response;
            }

            long amountInPaise = (long)(order.TotalAmount * 100);

            try
            {
                // 5️⃣ Create Razorpay order
                var client = new RazorpayClient(_keyId, _keySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", $"order_{order.OrderNumber}" }
                };

                var razorOrder = client.Order.Create(options);

                // 6️⃣ Save payment record
                var payment = new Models.Payment
                {
                    OrderNumber = order.OrderNumber,
                    RazorpayOrderId = razorOrder["id"],
                    AmountInPaise = amountInPaise,
                    Status = "Pending"
                };

                await _paymentRepo.CreateAsync(payment);

                response.Message = "Payment initiated successfully";
                response.Data = new PaymentResponseDto
                {
                    OrderNumber = payment.OrderNumber,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    AmountInPaise = payment.AmountInPaise
                };

                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to initiate payment";
                return response;
            }
        }

        // ===============================
        // CONFIRM PAYMENT (OK)
        // ===============================
        public async Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(
            PaymentConfirmationDto dto)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSignature = BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLower();

            if (generatedSignature != dto.RazorpaySignature)
            {
                response.Success = false;
                response.Message = "Invalid Razorpay signature";
                return response;
            }

            var payment = await _paymentRepo.GetByRazorpayOrderIdAsync(dto.RazorpayOrderId);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "Payment record not found";
                return response;
            }

            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = "Paid";

            await _paymentRepo.UpdateAsync(payment);

            response.Message = "Payment confirmed successfully";
            response.Data = new PaymentResponseDto
            {
                OrderNumber = payment.OrderNumber,
                RazorpayOrderId = payment.RazorpayOrderId,
                AmountInPaise = payment.AmountInPaise
            };

            return response;
        }

        // ===============================
        // GET PAYMENT
        // ===============================
        public async Task<ServiceResponse<PaymentResponseDto>> GetPaymentByOrderNumberAsync(
            string orderNumber)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            var payment = await _paymentRepo.GetByOrderNumberAsync(orderNumber);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "Payment not found";
                return response;
            }

            response.Message = "Payment fetched successfully";
            response.Data = new PaymentResponseDto
            {
                OrderNumber = payment.OrderNumber,
                RazorpayOrderId = payment.RazorpayOrderId,
                AmountInPaise = payment.AmountInPaise
            };

            return response;
        }
    }
}
