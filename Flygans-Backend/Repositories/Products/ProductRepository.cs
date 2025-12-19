using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly FlyganDbContext _context;

        public ProductRepository(FlyganDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryIdAsync(int categoryId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Simplified search: Name only
        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            return await _context.Products
                .Where(p => !p.IsDeleted &&
                            p.Name.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
        }
    }
}
