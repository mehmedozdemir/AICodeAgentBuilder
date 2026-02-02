using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AIResponse audit entity.
/// </summary>
public sealed class AIResponseRepository : Repository<AIResponse>, IAIResponseRepository
{
    public AIResponseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AIResponse>> GetByStatusAsync(
        AIResponseStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AIResponse>> GetResponsesRequiringReviewAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Status == AIResponseStatus.RequiresReview)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AIResponse>> GetByContextAsync(
        string requestContext,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.RequestContext == requestContext)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AIResponse>> GetSuccessfulResponsesAsync(
        DateTime? fromDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(r => r.Status == AIResponseStatus.Validated);

        if (fromDate.HasValue)
        {
            query = query.Where(r => r.RequestedAt >= fromDate.Value);
        }

        return await query
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(int avgPromptTokens, int avgCompletionTokens)> GetAverageTokenUsageAsync(
        CancellationToken cancellationToken = default)
    {
        var responses = await DbSet
            .Where(r => r.TokenCount.HasValue)
            .ToListAsync(cancellationToken);

        if (!responses.Any())
        {
            return (0, 0);
        }

        // Since AIResponse only has TokenCount, return it for both values
        var avgToken = (int)responses.Average(r => r.TokenCount ?? 0);

        return (avgToken, avgToken);
    }
}
