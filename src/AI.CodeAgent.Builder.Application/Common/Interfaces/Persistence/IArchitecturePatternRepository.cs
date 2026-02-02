using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;

/// <summary>
/// Repository interface for ArchitecturePattern aggregate root.
/// Defines architecture pattern-specific query and persistence operations.
/// </summary>
public interface IArchitecturePatternRepository : IRepository<ArchitecturePattern>
{
    /// <summary>
    /// Retrieves all architecture patterns ordered by name.
    /// </summary>
    Task<IEnumerable<ArchitecturePattern>> GetAllPatternsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves patterns suitable for a specific team size.
    /// </summary>
    Task<IEnumerable<ArchitecturePattern>> GetPatternsByTeamSizeAsync(
        bool forSmallTeam,
        bool forMediumTeam,
        bool forLargeTeam,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves patterns by complexity level range.
    /// </summary>
    Task<IEnumerable<ArchitecturePattern>> GetPatternsByComplexityAsync(
        int minComplexity,
        int maxComplexity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a pattern by its unique name.
    /// </summary>
    Task<ArchitecturePattern?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a pattern with the given name exists.
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
