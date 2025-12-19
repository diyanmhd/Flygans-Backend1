using Flygans_Backend.DTOs.Product;
using Flygans_Backend.Helpers;
using Flygans_Backend.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flygans_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // ----------------------------------------------------------
        // GET ALL PRODUCTS
        // ----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllProductsAsync();
            return Ok(response);
        }

        // ----------------------------------------------------------
        // GET BY ID
        // ----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetProductByIdAsync(id);
            return Ok(response);
        }

        // ----------------------------------------------------------
        // GET BY CATEGORY
        // ----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var response = await _service.GetProductsByCategoryIdAsync(categoryId);
            return Ok(response);
        }

        // ----------------------------------------------------------
        // SEARCH BY NAME ONLY
        // ----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _service.SearchProductsAsync(keyword);
            return Ok(response);
        }

        // ----------------------------------------------------------
        // CREATE PRODUCT WITH CLOUDINARY
        // ----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var response = await _service.CreateProductAsync(dto);
            return Ok(response);
        }

        // ----------------------------------------------------------
        // UPDATE PRODUCT 
        // ----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateProductDto dto)
        {
            var response = await _service.UpdateProductAsync(dto);
            return Ok(response);
        }

        // ----------------------------------------------------------
        // SOFT DELETE PRODUCT
        // ----------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteProductAsync(id);
            return Ok(response);
        }
    }
}
