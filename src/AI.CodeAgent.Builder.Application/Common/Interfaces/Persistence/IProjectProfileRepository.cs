using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;

/// <summary>
/// Repository interface for ProjectProfile aggregate root.
/// Defines project profile-specific query and persistence operations.
/// </summary>
public interface IProjectProfileRepository : IRepository<ProjectProfile>
{
    /// <summary>
    /// Retrieves a project profile with all related entities loaded (tech stacks, patterns, rules).
    /// </summary>
    Task<ProjectProfile?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all project profiles ordered by most recently updated.
    /// </summary>
    Task<IEnumerable<ProjectProfile>> GetAllProfilesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a profile by its unique project name.
    /// </summary>
    Task<ProjectProfile?> GetByProjectNameAsync(string projectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a profile with the given project name exists.
    /// </summary>
    Task<bool> ExistsByProjectNameAsync(string projectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves profiles that use a specific tech stack.
    /// </summary>
    Task<IEnumerable<ProjectProfile>> GetProfilesByTechStackAsync(
        Guid techStackId,
        CancellationToken cancellationToken = default);
}
