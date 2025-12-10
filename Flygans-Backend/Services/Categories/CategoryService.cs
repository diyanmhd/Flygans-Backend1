using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Categories;

namespace Flygans_Backend.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Category>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<Category?> GetById(int id)
    {
        return await _repo.GetById(id);
    }
}
