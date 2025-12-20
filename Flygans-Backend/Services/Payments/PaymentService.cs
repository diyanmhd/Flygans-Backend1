using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
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

        // ================================
        // INITIATE PAYMENT  (FIXED)
        // ================================
        public async Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(
            CreatePaymentRequestDto dto, int userId)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            // 1️⃣ Validate order
            var order = await _orderRepo.GetOrderByOrderNumberAsync(dto.OrderNumber);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }

            // 2️⃣ Check ownership
            if (order.UserId != userId)
            {
                response.Success = false;
                response.Message = "Unauthorized order access";
                return response;
            }

            // 3️⃣ Block only if order already paid
            if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Delivered)
            {
                response.Success = false;
                response.Message = "Order already paid";
                return response;
            }

            // 4️⃣ Validate amount
            if (order.TotalAmount <= 0)
            {
                response.Success = false;
                response.Message = "Invalid amount";
                return response;
            }

            long amountInPaise = (long)(order.TotalAmount * 100);

            try
            {
                var client = new RazorpayClient(_keyId, _keySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", $"order_{order.OrderNumber}" }
                };

                var razorOrder = client.Order.Create(options);

                // Save payment record
                var payment = new Flygans_Backend.Models.Payment
                {
                    OrderNumber = order.OrderNumber,
                    RazorpayOrderId = razorOrder["id"],
                    AmountInPaise = amountInPaise,
                    Status = "Pending"
                };

                await _paymentRepo.CreateAsync(payment);

                response.Success = true;
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

        // ================================
        // CONFIRM PAYMENT (FIXED)
        // ================================
        public async Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(
            PaymentConfirmationDto dto)
        {
            var response = new ServiceResponse<PaymentResponseDto>();

            string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generated = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (generated != dto.RazorpaySignature)
            {
                response.Success = false;
                response.Message = "Invalid signature";
                return response;
            }

            var payment = await _paymentRepo.GetByRazorpayOrderIdAsync(dto.RazorpayOrderId);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "Payment not found";
                return response;
            }

            // update payment
            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = "Paid";
            await _paymentRepo.UpdateAsync(payment);

            // update order status (using correct repo method)
            var order = await _orderRepo.GetOrderByOrderNumberAsync(payment.OrderNumber);
            if (order != null)
            {
                await _orderRepo.UpdateOrderStatus(order.Id, OrderStatus.Confirmed);
            }

            response.Success = true;
            response.Message = "Payment confirmed successfully";
            response.Data = new PaymentResponseDto
            {
                OrderNumber = payment.OrderNumber,
                RazorpayOrderId = payment.RazorpayOrderId,
                AmountInPaise = payment.AmountInPaise
            };

            return response;
        }

        // ================================
        // GET PAYMENT
        // ================================
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

            response.Success = true;
            response.Message = "Payment found";
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
