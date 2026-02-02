using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;

/// <summary>
/// Repository interface for AIResponse audit entity.
/// Tracks all AI interactions for traceability and analysis.
/// </summary>
public interface IAIResponseRepository : IRepository<AIResponse>
{
    /// <summary>
    /// Retrieves all AI responses with a specific status.
    /// </summary>
    Task<IEnumerable<AIResponse>> GetByStatusAsync(
        AIResponseStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves AI responses that require manual review.
    /// </summary>
    Task<IEnumerable<AIResponse>> GetResponsesRequiringReviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves AI responses by prompt context (e.g., "GenerateCategories", "GenerateTechStacks").
    /// </summary>
    Task<IEnumerable<AIResponse>> GetByContextAsync(
        string requestContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves successful AI responses for analytics.
    /// </summary>
    Task<IEnumerable<AIResponse>> GetSuccessfulResponsesAsync(
        DateTime? fromDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates average token usage across AI responses.
    /// </summary>
    Task<(int avgPromptTokens, int avgCompletionTokens)> GetAverageTokenUsageAsync(
        CancellationToken cancellationToken = default);
}
