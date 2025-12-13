namespace Flygans_Backend.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public long AmountInPaise { get; set; }
    }
}
