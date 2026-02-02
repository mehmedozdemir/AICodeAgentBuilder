using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ProjectProfile aggregate root.
/// </summary>
public sealed class ProjectProfileRepository : Repository<ProjectProfile>, IProjectProfileRepository
{
    public ProjectProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProjectProfile?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.TechStacks)
            .Include(p => p.ArchitecturePatternIds)
            .Include(p => p.EngineeringRuleIds)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProjectProfile>> GetAllProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectProfile?> GetByProjectNameAsync(string projectName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.ProjectName == projectName, cancellationToken);
    }

    public async Task<bool> ExistsByProjectNameAsync(string projectName, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(p => p.ProjectName == projectName, cancellationToken);
    }

    public async Task<IEnumerable<ProjectProfile>> GetProfilesByTechStackAsync(
        Guid techStackId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.TechStacks.Any(ts => ts.TechStackId == techStackId))
            .ToListAsync(cancellationToken);
    }
}
