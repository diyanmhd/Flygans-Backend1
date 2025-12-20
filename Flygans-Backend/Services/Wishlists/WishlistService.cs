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

    public async Task<ServiceResponse<bool>> AddToWishlist(int userId, int productId)
    {
        var response = new ServiceResponse<bool>();

        var exists = await _repo.GetByUserAndProduct(userId, productId);

        if (exists != null)
        {
            response.Success = false;
            response.Message = "Product already in wishlist";
            response.Data = false;
            return response;
        }

        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        await _repo.Add(wishlist);
        await _repo.Save();

        response.Success = true;
        response.Message = "Added to wishlist successfully";
        response.Data = true;

        return response;
    }

    public async Task<ServiceResponse<bool>> RemoveFromWishlist(int userId, int productId)
    {
        var response = new ServiceResponse<bool>();

        var wishlist = await _repo.GetByUserAndProduct(userId, productId);

        if (wishlist == null)
        {
            response.Success = false;
            response.Message = "Item not found in wishlist";
            response.Data = false;
            return response;
        }

        await _repo.Remove(wishlist);
        await _repo.Save();

        response.Success = true;
        response.Message = "Removed from wishlist";
        response.Data = true;

        return response;
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
