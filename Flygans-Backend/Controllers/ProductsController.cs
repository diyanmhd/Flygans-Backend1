using Microsoft.AspNetCore.Mvc;
using Flygans_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly FlyganDbContext _context;

        public ProductsController(FlyganDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
    }
}
