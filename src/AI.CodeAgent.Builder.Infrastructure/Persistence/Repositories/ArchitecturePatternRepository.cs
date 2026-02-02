using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ArchitecturePattern aggregate root.
/// </summary>
public sealed class ArchitecturePatternRepository : Repository<ArchitecturePattern>, IArchitecturePatternRepository
{
    public ArchitecturePatternRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ArchitecturePattern>> GetAllPatternsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchitecturePattern>> GetPatternsByTeamSizeAsync(
        bool forSmallTeam,
        bool forMediumTeam,
        bool forLargeTeam,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p =>
                (forSmallTeam && p.SuitableForSmallTeams) ||
                (forMediumTeam && p.SuitableForSmallTeams) ||
                (forLargeTeam && p.SuitableForLargeScale))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchitecturePattern>> GetPatternsByComplexityAsync(
        int minComplexity,
        int maxComplexity,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.ComplexityLevel >= minComplexity && p.ComplexityLevel <= maxComplexity)
            .OrderBy(p => p.ComplexityLevel)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArchitecturePattern?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(p => p.Name == name, cancellationToken);
    }
}
