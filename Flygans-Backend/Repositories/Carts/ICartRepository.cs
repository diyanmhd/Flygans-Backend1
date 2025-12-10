using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Carts;

public interface ICartRepository
{
    Task<Cart?> GetByUser(int userId);
    Task<Cart> Create(int userId);

    Task<CartItem?> GetItem(int cartId, int productId);

    Task AddItem(CartItem item);
    Task RemoveItem(CartItem item);

    // ✅ ADD THIS
    Task<List<CartItem>> GetItemsByCart(int cartId);

    Task Save();
}
