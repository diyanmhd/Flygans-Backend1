using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Wishlists;

public interface IWishlistRepository
{
    Task Add(Wishlist wishlist);
    Task Remove(Wishlist wishlist);
    Task<Wishlist?> GetByUserAndProduct(int userId, int productId);
    Task<List<Wishlist>> GetByUser(int userId);
    Task Save();
}
