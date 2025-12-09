using Flygans_Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // ✅ PROTECTS ALL ENDPOINTS IN THIS CONTROLLER
public class ProductsController : ControllerBase
{
    private readonly FlyganDbContext _context;

    public ProductsController(FlyganDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
}
