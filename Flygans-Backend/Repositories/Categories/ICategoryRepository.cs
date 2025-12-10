using Flygans_Backend.Models;

namespace Flygans_Backend.Repositories.Categories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAll();
    Task<Category?> GetById(int id);
}
