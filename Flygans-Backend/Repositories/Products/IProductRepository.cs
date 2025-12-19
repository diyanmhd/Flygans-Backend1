using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Products
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Product product);
        Task<Product?> GetByIdAsync(int id);

        Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20);

        Task<List<Product>> GetByCategoryIdAsync(int categoryId, int pageNumber = 1, int pageSize = 20);

        // Updated search signature
        Task<List<Product>> SearchProductsAsync(string keyword);
    }
}
