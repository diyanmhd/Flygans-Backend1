using Flygans_Backend.Dtos.Wishlist;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Wishlists;

public interface IWishlistService
{
    Task<ServiceResponse<bool>> AddToWishlist(int userId, int productId);
    Task<ServiceResponse<bool>> RemoveFromWishlist(int userId, int productId);

    Task<ServiceResponse<List<WishlistDto>>> GetWishlist(int userId);
}
