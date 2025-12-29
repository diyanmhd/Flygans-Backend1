using Flygans_Backend.DTOs.Payments;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Payments;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using Flygans_Backend.Exceptions;

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
            _keyId = config["Razorpay:KeyId"]!;
            _keySecret = config["Razorpay:KeySecret"]!;
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
        }

        // ================= INITIATE PAYMENT =================
        public async Task<ServiceResponse<PaymentResponseDto>> InitiatePaymentAsync(
            CreatePaymentRequestDto dto, int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(dto.OrderId);

            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.UserId != userId)
                throw new UnauthorizedException("Unauthorized order access");

            if (order.Status != OrderStatus.PendingPayment)
                throw new BadRequestException("Order not eligible for payment");

            if (order.TotalAmount <= 0)
                throw new BadRequestException("Invalid amount");

            long amountInPaise = (long)(order.TotalAmount * 100);

            var client = new RazorpayClient(_keyId, _keySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "receipt", $"order_{order.OrderNumber}" }
            };

            var razorOrder = client.Order.Create(options);

            // ✅ FIX: explicitly use YOUR Payment entity
            var payment = new Flygans_Backend.Models.Payment
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                RazorpayOrderId = razorOrder["id"].ToString()!,
                AmountInPaise = amountInPaise,
                Status = "Pending"
            };

            await _paymentRepo.CreateAsync(payment);

            return new ServiceResponse<PaymentResponseDto>
            {
                Success = true,
                Message = "Payment initiated successfully",
                Data = new PaymentResponseDto
                {
                    OrderNumber = payment.OrderNumber,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    AmountInPaise = payment.AmountInPaise
                }
            };
        }

        // ================= CONFIRM PAYMENT =================
        public async Task<ServiceResponse<PaymentResponseDto>> ConfirmPaymentAsync(
            PaymentConfirmationDto dto)
        {
            string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generated = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (generated != dto.RazorpaySignature)
                throw new BadRequestException("Invalid signature");

            var payment = await _paymentRepo
                .GetByRazorpayOrderIdAsync(dto.RazorpayOrderId);

            if (payment == null)
                throw new NotFoundException("Payment not found");

            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = "Paid";

            await _paymentRepo.UpdateAsync(payment);

            await _orderRepo.UpdateOrderStatus(payment.OrderId, OrderStatus.Confirmed);

            return new ServiceResponse<PaymentResponseDto>
            {
                Success = true,
                Message = "Payment confirmed successfully",
                Data = new PaymentResponseDto
                {
                    OrderNumber = payment.OrderNumber,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    AmountInPaise = payment.AmountInPaise
                }
            };
        }

        // ================= GET PAYMENT =================
        public async Task<ServiceResponse<PaymentResponseDto>> GetPaymentByOrderNumberAsync(
            string orderNumber)
        {
            var payment = await _paymentRepo.GetByOrderNumberAsync(orderNumber);

            if (payment == null)
                throw new NotFoundException("Payment not found");

            return new ServiceResponse<PaymentResponseDto>
            {
                Success = true,
                Message = "Payment found",
                Data = new PaymentResponseDto
                {
                    OrderNumber = payment.OrderNumber,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    AmountInPaise = payment.AmountInPaise
                }
            };
        }
    }
}
