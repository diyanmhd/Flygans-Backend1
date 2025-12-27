using Flygans_Backend.Data;
using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Services.Cloudinary;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Flygans_Backend.Exceptions; // required

namespace Flygans_Backend.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICloudinaryService _cloudinary;
        private readonly FlyganDbContext _context;

        public ProductService(IProductRepository repo, ICloudinaryService cloudinary, FlyganDbContext context)
        {
            _repo = repo;
            _cloudinary = cloudinary;
            _context = context;
        }

        // CREATE WITH DUPLICATE CHECK
        public async Task<ServiceResponse<string>> CreateProductAsync(CreateProductDto dto)
        {
            // normalize name to compare
            var normalizedName = Regex.Replace(dto.Name, @"\s+", "").Trim().ToLower();

            var dbNames = await _context.Products
                .Where(p => !p.IsDeleted)
                .Select(p => p.Name)
                .ToListAsync();

            var exists = dbNames.Any(name =>
                Regex.Replace(name, @"\s+", "").Trim().ToLower() == normalizedName
            );

            if (exists)
                throw new BadRequestException("Product with same or similar name already exists");

            var cleanName = Regex.Replace(dto.Name, @"\s+", " ").Trim();

            var imageUrl = await _cloudinary.UploadImageAsync(dto.Image);
            var publicId = imageUrl.Split('/').Last().Split('.').First();

            var product = new Product
            {
                Name = cleanName,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                StockQuantity = dto.StockQuantity,
                ImageUrl = imageUrl,
                PublicId = publicId,
                IsDeleted = false
            };

            await _repo.CreateAsync(product);

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Product created successfully"
            };
        }

        // UPDATE
        public async Task<ServiceResponse<string>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _repo.GetByIdAsync(dto.Id);

            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product not found");

            product.Name = Regex.Replace(dto.Name, @"\s+", " ").Trim();
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.StockQuantity = dto.StockQuantity;

            if (dto.Image != null)
            {
                await _cloudinary.DeleteImageAsync(product.PublicId);

                var newImageUrl = await _cloudinary.UploadImageAsync(dto.Image);
                product.ImageUrl = newImageUrl;
                product.PublicId = newImageUrl.Split('/').Last().Split('.').First();
            }

            await _repo.UpdateAsync(product);

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Product updated successfully"
            };
        }

        // DELETE soft
        public async Task<ServiceResponse<bool>> DeleteProductAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                throw new NotFoundException("Product not found");

            product.IsDeleted = true;

            await _cloudinary.DeleteImageAsync(product.PublicId);
            await _repo.UpdateAsync(product);

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Product deleted",
                Data = true
            };
        }

        // GET ALL
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _repo.GetAllProductsAsync();

            return new ServiceResponse<IEnumerable<ProductDto>>
            {
                Success = true,
                Data = products
                    .Where(p => !p.IsDeleted)
                    .Select(Map)
                    .ToList()
            };
        }

        // GET BY ID
        public async Task<ServiceResponse<ProductDto?>> GetProductByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product not found");

            return new ServiceResponse<ProductDto?>
            {
                Success = true,
                Data = Map(product)
            };
        }

        // CATEGORY FILTER
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);

            if (!categoryExists)
                throw new NotFoundException("Category does not exist");

            var products = await _repo.GetByCategoryIdAsync(categoryId);

            if (products.Count == 0)
                throw new NotFoundException("No products found in this category");

            return new ServiceResponse<IEnumerable<ProductDto>>
            {
                Success = true,
                Data = products
                    .Where(p => !p.IsDeleted)
                    .Select(Map)
            };
        }

        // SEARCH
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadRequestException("Keyword is required");

            var products = await _repo.SearchProductsAsync(keyword);

            if (products.Count == 0)
                throw new NotFoundException("No products found");

            return new ServiceResponse<IEnumerable<ProductDto>>
            {
                Success = true,
                Data = products
                    .Where(p => !p.IsDeleted)
                    .Select(Map)
            };
        }

        private ProductDto Map(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl
            };
        }
    }
}
