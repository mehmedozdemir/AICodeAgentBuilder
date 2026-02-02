using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for EngineeringRule aggregate root.
/// </summary>
public sealed class EngineeringRuleRepository : Repository<EngineeringRule>, IEngineeringRuleRepository
{
    public EngineeringRuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EngineeringRule>> GetAllActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Constraint.Severity)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EngineeringRule>> GetRulesBySeverityAsync(
        RuleSeverity severity,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Constraint.Severity == severity)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EngineeringRule>> GetRulesByScopeAsync(
        RuleScope scope,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Constraint.Scope == scope)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<EngineeringRule?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<EngineeringRule>> GetPotentialConflictsAsync(
        EngineeringRule rule,
        CancellationToken cancellationToken = default)
    {
        // Simple implementation: find rules with same scope but different severity
        // In a real implementation, this would use more sophisticated conflict detection
        return await DbSet
            .Where(r => r.Id != rule.Id && r.Constraint.Scope == rule.Constraint.Scope && r.IsActive)
            .ToListAsync(cancellationToken);
    }
}
