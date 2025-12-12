namespace Flygans_Backend.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public long AmountInPaise { get; set; }
        public bool Paid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
