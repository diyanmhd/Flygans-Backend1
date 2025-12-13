using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task UpdateAsync(Payment payment);

        Task<Payment?> GetByOrderNumberAsync(string orderNumber);

        // ✅ NEW — needed for Razorpay confirmation
        Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId);
    }
}
