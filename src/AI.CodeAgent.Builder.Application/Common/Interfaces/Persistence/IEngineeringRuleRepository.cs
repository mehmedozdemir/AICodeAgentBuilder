using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;

/// <summary>
/// Repository interface for EngineeringRule aggregate root.
/// Defines engineering rule-specific query and persistence operations.
/// </summary>
public interface IEngineeringRuleRepository : IRepository<EngineeringRule>
{
    /// <summary>
    /// Retrieves all active engineering rules ordered by severity.
    /// </summary>
    Task<IEnumerable<EngineeringRule>> GetAllActiveRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves rules by severity level.
    /// </summary>
    Task<IEnumerable<EngineeringRule>> GetRulesBySeverityAsync(
        RuleSeverity severity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves rules by scope.
    /// </summary>
    Task<IEnumerable<EngineeringRule>> GetRulesByScopeAsync(
        RuleScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a rule by its unique name.
    /// </summary>
    Task<EngineeringRule?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a rule with the given name exists.
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves rules that may conflict with the specified rule.
    /// </summary>
    Task<IEnumerable<EngineeringRule>> GetPotentialConflictsAsync(
        EngineeringRule rule,
        CancellationToken cancellationToken = default);
}
