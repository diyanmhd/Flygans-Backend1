using Flygans_Backend.Data;
using Flygans_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Repositories.Products;

public class ProductRepository : IProductRepository
{
    private readonly FlyganDbContext _context;

    public ProductRepository(FlyganDbContext context)
    {
        _context = context;
    }

    public async Task Add(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    // ✅ GET ALL PRODUCTS
    public async Task<List<Product>> GetAll()
    {
        return await _context.Products.ToListAsync();
    }
}
