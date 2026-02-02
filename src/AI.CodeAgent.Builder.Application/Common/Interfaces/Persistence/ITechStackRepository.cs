using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;

/// <summary>
/// Repository interface for TechStack aggregate root.
/// Defines tech stack-specific query and persistence operations.
/// </summary>
public interface ITechStackRepository : IRepository<TechStack>
{
    /// <summary>
    /// Retrieves all tech stacks for a specific category, including their parameters.
    /// </summary>
    Task<IEnumerable<TechStack>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tech stack by its unique name within a category.
    /// </summary>
    Task<TechStack?> GetByNameAsync(Guid categoryId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tech stack with all its parameters loaded.
    /// </summary>
    Task<TechStack?> GetWithParametersAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tech stack with the given name exists within a category.
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid categoryId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active tech stacks ordered by popularity.
    /// </summary>
    Task<IEnumerable<TechStack>> GetPopularTechStacksAsync(int count, CancellationToken cancellationToken = default);
}
