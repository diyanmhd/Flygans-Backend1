using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Carts;

public class CartRepository : ICartRepository
{
    private readonly FlyganDbContext _context;

    public CartRepository(FlyganDbContext context)
    {
        _context = context;
    }

    // ✅ Get cart by user (DO NOT LOAD USER)
    public async Task<Cart?> GetByUser(int userId)
    {
        return await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);  // ✅ FIXED
    }

    // ✅ Create new cart
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

    // ✅ Get a specific cart item
    public async Task<CartItem?> GetItem(int cartId, int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(i =>
                i.CartId == cartId &&
                i.ProductId == productId
            );
    }

    // ✅ Add item to cart
    public async Task AddItem(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
    }

    // ✅ Remove item from cart
    public Task RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
        return Task.CompletedTask;
    }

    // ✅ GET ALL ITEMS IN CART
    public async Task<List<CartItem>> GetItemsByCart(int cartId)
    {
        return await _context.CartItems
            .Include(i => i.Product)   // ✅ Only product data is needed
            .Where(i => i.CartId == cartId)
            .ToListAsync();
    }

    // ✅ Save changes
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
