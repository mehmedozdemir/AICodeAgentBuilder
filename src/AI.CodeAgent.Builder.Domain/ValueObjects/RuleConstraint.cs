using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Domain.ValueObjects;

/// <summary>
/// Value object representing a rule constraint.
/// Encapsulates the severity and scope of an engineering rule.
/// Immutable by design to prevent accidental modification.
/// </summary>
public sealed class RuleConstraint : ValueObject
{
    private RuleConstraint(RuleSeverity severity, RuleScope scope)
    {
        Severity = severity;
        Scope = scope;
    }

    public RuleSeverity Severity { get; }
    public RuleScope Scope { get; }

    public static RuleConstraint Create(RuleSeverity severity, RuleScope scope)
    {
        return new RuleConstraint(severity, scope);
    }

    /// <summary>
    /// Checks if this rule is more restrictive than another.
    /// Critical > Error > Warning > Info
    /// </summary>
    public bool IsMoreRestrictiveThan(RuleConstraint other)
    {
        return Severity > other.Severity;
    }

    /// <summary>
    /// Checks if this rule applies to the given scope.
    /// Global rules apply to all scopes.
    /// </summary>
    public bool AppliesTo(RuleScope targetScope)
    {
        return Scope == RuleScope.Global || Scope == targetScope;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Severity;
        yield return Scope;
    }

    public override string ToString() => $"{Severity} - {Scope}";
}
