using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Models;

namespace Flygans_Backend.Services.Products;

public interface IProductService
{
    Task AddProduct(CreateProductDto dto);

    // ✅ GET ALL PRODUCTS
    Task<List<Product>> GetProducts();
}
