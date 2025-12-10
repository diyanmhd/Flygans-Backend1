using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Products;

public interface IProductRepository
{
    Task Add(Product product);
    Task Save();

    // ✅ GET ALL PRODUCTS
    Task<List<Product>> GetAll();
}
