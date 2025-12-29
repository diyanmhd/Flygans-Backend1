using System.Text.Json.Serialization;

namespace Flygans_Backend.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        PendingPayment, // Razorpay order created, payment not yet done
        COD,            // Cash on Delivery
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
