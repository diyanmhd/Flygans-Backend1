using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Wishlists;

namespace Flygans_Backend.Services.Wishlists;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _repo;

    public WishlistService(IWishlistRepository repo)
    {
        _repo = repo;
    }

    public async Task AddToWishlist(int userId, int productId)
    {
        var exists = await _repo.GetByUserAndProduct(userId, productId);

        if (exists != null)
            return; // already in wishlist

        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        await _repo.Add(wishlist);
        await _repo.Save();
    }

    public async Task RemoveFromWishlist(int userId, int productId)
    {
        var wishlist = await _repo.GetByUserAndProduct(userId, productId);

        if (wishlist == null)
            return;

        await _repo.Remove(wishlist);
        await _repo.Save();
    }

    public async Task<List<Wishlist>> GetWishlist(int userId)
    {
        return await _repo.GetByUser(userId);
    }
}
