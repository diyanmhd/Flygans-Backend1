using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Wishlists;
using Flygans_Backend.Dtos.Wishlist;
using Flygans_Backend.Helpers;

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
            return;

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

    public async Task<ServiceResponse<List<WishlistDto>>> GetWishlist(int userId)
    {
        var wishlistItems = await _repo.GetByUser(userId);

        var data = wishlistItems.Select(w => new WishlistDto
        {
            Id = w.Id,
            ProductId = w.ProductId,
            Product = new ProductDto
            {
                Id = w.Product.Id,
                Name = w.Product.Name,
                Price = w.Product.Price,
                StockQuantity = w.Product.StockQuantity,
                ImageUrl = w.Product.ImageUrl
            }
        }).ToList();

        return new ServiceResponse<List<WishlistDto>>
        {
            Success = true,
            Message = "Wishlist fetched successfully",
            Data = data
        };
    }
}
