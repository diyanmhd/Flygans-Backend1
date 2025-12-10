using Flygans_Backend.DTOs.Cart;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Carts;

namespace Flygans_Backend.Services.Carts;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    // ✅ ADD TO CART
    public async Task AddToCart(int userId, int productId, int quantity)
    {
        var cart = await _repo.GetByUser(userId);

        if (cart == null)
            cart = await _repo.Create(userId);

        var item = await _repo.GetItem(cart.Id, productId);

        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            item = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };

            await _repo.AddItem(item);
        }

        await _repo.Save();
    }

    // ✅ REMOVE SINGLE ITEM
    public async Task RemoveFromCart(int userId, int productId)
    {
        var cart = await _repo.GetByUser(userId);
        if (cart == null) return;

        var item = await _repo.GetItem(cart.Id, productId);
        if (item == null) return;

        await _repo.RemoveItem(item);
        await _repo.Save();
    }

    // ✅ GET CART ITEMS — SAFE DTO
    public async Task<List<CartItemDto>> GetCartItems(int userId)
    {
        var cart = await _repo.GetByUser(userId);

        if (cart == null)
            return new List<CartItemDto>();

        var items = await _repo.GetItemsByCart(cart.Id);

        return items.Select(i => new CartItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            Price = i.Product.Price,

            // ✅ FIX: Category object → Category name string
            Category = i.Product.Category.Name,

            ImageUrl = i.Product.ImageUrl,
            Quantity = i.Quantity
        }).ToList();
    }

    // ✅ UPDATE ITEM QUANTITY
    public async Task UpdateQuantity(int userId, int productId, int quantity)
    {
        var cart = await _repo.GetByUser(userId);
        if (cart == null) return;

        var item = await _repo.GetItem(cart.Id, productId);
        if (item == null) return;

        item.Quantity = quantity;
        await _repo.Save();
    }

    // ✅ CLEAR ENTIRE CART
    public async Task ClearCart(int userId)
    {
        var cart = await _repo.GetByUser(userId);
        if (cart == null) return;

        var items = await _repo.GetItemsByCart(cart.Id);

        foreach (var item in items)
        {
            await _repo.RemoveItem(item);
        }

        await _repo.Save();
    }
}
