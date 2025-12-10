using Flygans_Backend.DTOs.Cart;

namespace Flygans_Backend.Services.Carts;

public interface ICartService
{
    Task AddToCart(int userId, int productId, int quantity);

    // ✅ Remove single item
    Task RemoveFromCart(int userId, int productId);

    // ✅ Get cart items (SAFE DTO)
    Task<List<CartItemDto>> GetCartItems(int userId);

    // ✅ UPDATE quantity of one cart item
    Task UpdateQuantity(int userId, int productId, int quantity);

    // ✅ CLEAR whole cart
    Task ClearCart(int userId);
}
