using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Products;

namespace Flygans_Backend.Services.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task AddProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl
        };

        await _repo.Add(product);
        await _repo.Save();
    }

    // ✅ GET ALL PRODUCTS
    public async Task<List<Product>> GetProducts()
    {
        return await _repo.GetAll();
    }
}
