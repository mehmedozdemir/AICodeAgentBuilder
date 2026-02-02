using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TechStack aggregate root.
/// </summary>
public sealed class TechStackRepository : Repository<TechStack>, ITechStackRepository
{
    public TechStackRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TechStack>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ts => ts.Parameters)
            .Where(ts => ts.CategoryId == categoryId)
            .OrderBy(ts => ts.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TechStack?> GetByNameAsync(Guid categoryId, string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(ts => ts.CategoryId == categoryId && ts.Name == name, cancellationToken);
    }

    public async Task<TechStack?> GetWithParametersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ts => ts.Parameters)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid categoryId, string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(ts => ts.CategoryId == categoryId && ts.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<TechStack>> GetPopularTechStacksAsync(int count, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ts => ts.IsActive)
            .OrderBy(ts => ts.Name)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
