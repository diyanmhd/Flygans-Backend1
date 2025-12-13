using Flygans_Backend.Dtos.Wishlist;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Wishlists;

public interface IWishlistService
{
    Task AddToWishlist(int userId, int productId);
    Task RemoveFromWishlist(int userId, int productId);

    // STANDARD RESPONSE
    Task<ServiceResponse<List<WishlistDto>>> GetWishlist(int userId);
}
