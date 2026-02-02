namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when engineering rules conflict with each other.
/// Examples: mutually exclusive requirements, contradictory severity levels.
/// </summary>
public sealed class RuleConflictException : DomainException
{
    public RuleConflictException(string rule1Name, string rule2Name, string conflictReason)
        : base($"Engineering rules '{rule1Name}' and '{rule2Name}' conflict: {conflictReason}")
    {
        Rule1Name = rule1Name;
        Rule2Name = rule2Name;
        ConflictReason = conflictReason;
    }

    public string Rule1Name { get; }
    public string Rule2Name { get; }
    public string ConflictReason { get; }
}
