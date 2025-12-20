using Flygans_Backend.DTOs.Cart;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Carts;
using Flygans_Backend.Repositories.Users;

namespace Flygans_Backend.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;
        private readonly IAdminUserRepository _userRepo;

        public CartService(ICartRepository repo, IAdminUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        private async Task ValidateUser(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null || user.IsDeleted)
                throw new Exception("This account was deleted by an admin.");

            if (user.IsBlocked)
                throw new Exception("Your account is blocked. Contact admin.");
        }

        // ADD or increase quantity
        public async Task AddToCart(int userId, int productId, int quantity)
        {
            await ValidateUser(userId);

            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

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

        public async Task RemoveFromCart(int userId, int productId)
        {
            await ValidateUser(userId);

            var cart = await _repo.GetByUser(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = await _repo.GetItem(cart.Id, productId);

            if (item == null)
                throw new Exception("Item not found in cart");

            await _repo.RemoveItem(item);
            await _repo.Save();
        }

        public async Task<List<CartItemDto>> GetCartItems(int userId)
        {
            await ValidateUser(userId);

            var cart = await _repo.GetByUser(userId);

            if (cart == null)
                return new List<CartItemDto>();

            var items = await _repo.GetItemsByCart(cart.Id);

            return items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Price = i.Product.Price,
                Category = i.Product.Category.Name,
                ImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity
            }).ToList();
        }

        // UPDATE quantity or remove if <= 0
        public async Task UpdateQuantity(int userId, int productId, int quantity)
        {
            await ValidateUser(userId);

            var cart = await _repo.GetByUser(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = await _repo.GetItem(cart.Id, productId);

            if (item == null)
                throw new Exception("Item not found in cart");

            if (quantity <= 0)
            {
                await _repo.RemoveItem(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _repo.Save();
        }

        public async Task ClearCart(int userId)
        {
            await ValidateUser(userId);

            var cart = await _repo.GetByUser(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var items = await _repo.GetItemsByCart(cart.Id);

            foreach (var item in items)
            {
                await _repo.RemoveItem(item);
            }

            await _repo.Save();
        }
    }
}
