using Flygans_Backend.DTOs.Cart;

namespace Flygans_Backend.Services.Carts;

public interface ICartService
{
    Task AddToCart(int userId, int productId, int quantity);

    Task RemoveFromCart(int userId, int productId);

    Task<List<CartItemDto>> GetCartItems(int userId);

    Task UpdateQuantity(int userId, int productId, int quantity);

    Task ClearCart(int userId);
}
