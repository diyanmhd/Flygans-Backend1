using Flygans_Backend.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Flygans_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    // ✅ GET ALL CATEGORIES
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetAll();
        return Ok(categories);
    }

    // ✅ GET CATEGORY BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _service.GetById(id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }
}
