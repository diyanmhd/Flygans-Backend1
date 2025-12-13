using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly FlyganDbContext _context;

        public PaymentRepository(FlyganDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderNumber == orderNumber);
        }

        // ✅ NEW — Required for Razorpay confirmation
        public async Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.RazorpayOrderId == razorpayOrderId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
