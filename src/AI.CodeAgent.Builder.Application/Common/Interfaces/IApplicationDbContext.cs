namespace AI.CodeAgent.Builder.Application.Common.Interfaces;

/// <summary>
/// Interface for the database context.
/// This interface lives in the Application layer but is implemented in Infrastructure.
/// </summary>
public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
