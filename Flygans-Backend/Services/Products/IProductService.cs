using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Helpers;

namespace Flygans_Backend.Services.Products
{
    public interface IProductService
    {
        Task<ServiceResponse<string>> CreateProductAsync(CreateProductDto dto);

        Task<ServiceResponse<string>> UpdateProductAsync(UpdateProductDto dto);

        Task<ServiceResponse<bool>> DeleteProductAsync(int id);

        Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();

        Task<ServiceResponse<ProductDto?>> GetProductByIdAsync(int id);

        // UPDATED → validates category
        Task<ServiceResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId);

        // UPDATED → search by name only
        Task<ServiceResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string keyword);
    }
}
