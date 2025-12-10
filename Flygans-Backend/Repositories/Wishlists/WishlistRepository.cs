using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Wishlists;

public class WishlistRepository : IWishlistRepository
{
    private readonly FlyganDbContext _context;

    public WishlistRepository(FlyganDbContext context)
    {
        _context = context;
    }

    public async Task Add(Wishlist wishlist)
    {
        await _context.Wishlists.AddAsync(wishlist);
    }

    public Task Remove(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
        return Task.CompletedTask;
    }

    public async Task<Wishlist?> GetByUserAndProduct(int userId, int productId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w =>
                w.UserId == userId && w.ProductId == productId
            );
    }

    public async Task<List<Wishlist>> GetByUser(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
