namespace Flygans_Backend.DTOs.Payments
{
    public class PaymentConfirmationDto
    {
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;

        // Local reference (now using OrderNumber)
        public string OrderNumber { get; set; } = string.Empty;
    }
}
