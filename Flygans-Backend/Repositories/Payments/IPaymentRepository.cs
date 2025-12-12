using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByOrderNumberAsync(string orderNumber);
        Task UpdateAsync(Payment payment);
    }
}
