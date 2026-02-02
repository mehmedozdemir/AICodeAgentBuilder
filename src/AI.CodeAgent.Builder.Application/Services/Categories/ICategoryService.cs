using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.Application.Services.Categories;

/// <summary>
/// Service interface for category management operations.
/// </summary>
public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<Category> CreateCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid categoryId);
}
