using Flygans_Backend.Models;

namespace Flygans_Backend.Services.Wishlists;

public interface IWishlistService
{
    Task AddToWishlist(int userId, int productId);
    Task RemoveFromWishlist(int userId, int productId);
    Task<List<Wishlist>> GetWishlist(int userId);
}
