using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Category aggregate root.
/// </summary>
public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAIGeneratedCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsAIGenerated)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
