using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Carts
{
    public class CartRepository : ICartRepository
    {
        private readonly FlyganDbContext _context;

        public CartRepository(FlyganDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUser(int userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> Create(int userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<CartItem?> GetItem(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(i =>
                    i.CartId == cartId &&
                    i.ProductId == productId
                );
        }

        public async Task AddItem(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public Task RemoveItem(CartItem item)
        {
            _context.CartItems.Remove(item);
            return Task.CompletedTask;
        }

        public async Task<List<CartItem>> GetItemsByCart(int cartId)
        {
            return await _context.CartItems
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Where(i => i.CartId == cartId)
                .ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
