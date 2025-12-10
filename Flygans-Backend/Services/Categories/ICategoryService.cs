using Flygans_Backend.Models;

namespace Flygans_Backend.Services.Categories;

public interface ICategoryService
{
    Task<List<Category>> GetAll();
    Task<Category?> GetById(int id);
}
