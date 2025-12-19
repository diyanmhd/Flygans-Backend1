using Flygans_Backend.Data;
using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Services.Cloudinary;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        // CREATE WITH STRICT DUPLICATE CHECK
        public async Task<ServiceResponse<string>> CreateProductAsync(CreateProductDto dto)
        {
            var response = new ServiceResponse<string>();

            try
            {
                // 1️⃣ Normalize incoming name: remove ALL spaces + lowercase
                var normalizedName = Regex.Replace(dto.Name, @"\s+", "").Trim().ToLower();

                // 2️⃣ Fetch product names from DB (no regex used in SQL)
                var dbNames = await _context.Products
                    .Where(p => !p.IsDeleted)
                    .Select(p => p.Name)
                    .ToListAsync();

                // 3️⃣ Normalize existing names & check duplicates IN MEMORY
                var exists = dbNames.Any(name =>
                    Regex.Replace(name, @"\s+", "").Trim().ToLower() == normalizedName
                );

                if (exists)
                {
                    response.Success = false;
                    response.Message = "Product with same or similar name already exists";
                    return response;
                }

                // store name cleaned (trim & collapse spaces, not remove all)
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

                response.Message = "Product created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // UPDATE
        public async Task<ServiceResponse<string>> UpdateProductAsync(UpdateProductDto dto)
        {
            var response = new ServiceResponse<string>();

            var product = await _repo.GetByIdAsync(dto.Id);

            if (product == null || product.IsDeleted)
            {
                response.Success = false;
                response.Message = "Product not found";
                return response;
            }

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

            response.Message = "Product updated successfully";
            return response;
        }

        // DELETE (soft)
        public async Task<ServiceResponse<bool>> DeleteProductAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            var product = await _repo.GetByIdAsync(id);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Product not found";
                return response;
            }

            product.IsDeleted = true;

            await _cloudinary.DeleteImageAsync(product.PublicId);
            await _repo.UpdateAsync(product);

            response.Data = true;
            response.Message = "Product deleted";
            return response;
        }

        // GET ALL
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();

            var products = await _repo.GetAllProductsAsync();

            response.Data = products
                .Where(p => !p.IsDeleted)
                .Select(Map)
                .ToList();

            return response;
        }

        // GET BY ID
        public async Task<ServiceResponse<ProductDto?>> GetProductByIdAsync(int id)
        {
            var response = new ServiceResponse<ProductDto?>();

            var product = await _repo.GetByIdAsync(id);

            if (product == null || product.IsDeleted)
            {
                response.Success = false;
                response.Message = "Product not found";
                return response;
            }

            response.Data = Map(product);
            return response;
        }

        // CATEGORY FILTER
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);

            if (!categoryExists)
            {
                response.Success = false;
                response.Message = "Category not existing";
                return response;
            }

            var products = await _repo.GetByCategoryIdAsync(categoryId);

            if (products.Count == 0)
            {
                response.Success = false;
                response.Message = "No products found in this category";
                return response;
            }

            response.Data = products
                .Where(p => !p.IsDeleted)
                .Select(Map);

            return response;
        }

        // SEARCH
        public async Task<ServiceResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string keyword)
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                response.Success = false;
                response.Message = "Keyword is required";
                return response;
            }

            var products = await _repo.SearchProductsAsync(keyword);

            if (products.Count == 0)
            {
                response.Success = false;
                response.Message = "No products found";
                return response;
            }

            response.Data = products
                .Where(p => !p.IsDeleted)
                .Select(Map);

            return response;
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
