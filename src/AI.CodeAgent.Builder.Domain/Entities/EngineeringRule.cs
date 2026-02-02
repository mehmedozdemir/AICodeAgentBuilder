using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents a cross-cutting engineering constraint or best practice.
/// Examples: Unit tests required, Code coverage > 80%, No dynamic SQL, Use async/await.
/// 
/// EngineeringRule is an aggregate root that defines composable rules
/// used to generate copilot instructions and code agent constraints.
/// </summary>
public sealed class EngineeringRule : BaseEntity
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _rationale = string.Empty;
    private RuleConstraint _constraint = null!;

    // Private constructor for EF Core
    private EngineeringRule()
    {
    }

    private EngineeringRule(string name, string description, string rationale, RuleConstraint constraint)
    {
        SetName(name);
        SetDescription(description);
        SetRationale(rationale);
        _constraint = constraint;
        IsActive = true;
        IsEnforced = true;
    }

    /// <summary>
    /// The name of the engineering rule.
    /// Should be concise and descriptive.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// A detailed description of what this rule enforces.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// Explanation of why this rule exists and the problems it prevents.
    /// Used in generating educational content for code agents.
    /// </summary>
    public string Rationale
    {
        get => _rationale;
        private set => _rationale = value;
    }

    /// <summary>
    /// The constraint that defines severity and scope of this rule.
    /// </summary>
    public RuleConstraint Constraint
    {
        get => _constraint;
        private set => _constraint = value;
    }

    /// <summary>
    /// Specific guidance on how to implement or follow this rule.
    /// Examples: "Use xUnit for unit tests", "Maintain coverage using Coverlet".
    /// </summary>
    public string? ImplementationGuidance { get; private set; }

    /// <summary>
    /// Common violations or anti-patterns related to this rule.
    /// Used in generating code agent warnings.
    /// </summary>
    public string? CommonViolations { get; private set; }

    /// <summary>
    /// Example code or scenarios demonstrating compliance with this rule.
    /// </summary>
    public string? ExampleCode { get; private set; }

    /// <summary>
    /// Indicates whether this rule is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates whether violations of this rule should be strictly enforced.
    /// If false, the rule serves as a recommendation only.
    /// </summary>
    public bool IsEnforced { get; private set; }

    /// <summary>
    /// Indicates whether this rule was created by AI.
    /// </summary>
    public bool IsAIGenerated { get; private set; }

    /// <summary>
    /// Tags for categorization and filtering (comma-separated).
    /// Examples: "testing", "security", "performance", "maintainability".
    /// </summary>
    public string? Tags { get; private set; }

    /// <summary>
    /// Factory method to create a new engineering rule.
    /// </summary>
    public static EngineeringRule Create(
        string name,
        string description,
        string rationale,
        RuleConstraint constraint,
        bool isAIGenerated = false)
    {
        var rule = new EngineeringRule(name, description, rationale, constraint)
        {
            IsAIGenerated = isAIGenerated
        };

        return rule;
    }

    /// <summary>
    /// Updates the rule name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Engineering rule name cannot be empty.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Engineering rule name cannot exceed 200 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the rule description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Engineering rule description cannot be empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Engineering rule description cannot exceed 1000 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the rule rationale.
    /// </summary>
    public void SetRationale(string rationale)
    {
        if (string.IsNullOrWhiteSpace(rationale))
            throw new ArgumentException("Engineering rule rationale cannot be empty.", nameof(rationale));

        if (rationale.Length > 1000)
            throw new ArgumentException("Engineering rule rationale cannot exceed 1000 characters.", nameof(rationale));

        _rationale = rationale.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the rule constraint.
    /// </summary>
    public void SetConstraint(RuleConstraint constraint)
    {
        _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets implementation guidance.
    /// </summary>
    public void SetImplementationGuidance(string? guidance)
    {
        if (!string.IsNullOrWhiteSpace(guidance) && guidance.Length > 1000)
            throw new ArgumentException("Implementation guidance cannot exceed 1000 characters.", nameof(guidance));

        ImplementationGuidance = guidance?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets common violations.
    /// </summary>
    public void SetCommonViolations(string? violations)
    {
        if (!string.IsNullOrWhiteSpace(violations) && violations.Length > 1000)
            throw new ArgumentException("Common violations cannot exceed 1000 characters.", nameof(violations));

        CommonViolations = violations?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets example code.
    /// </summary>
    public void SetExampleCode(string? exampleCode)
    {
        if (!string.IsNullOrWhiteSpace(exampleCode) && exampleCode.Length > 5000)
            throw new ArgumentException("Example code cannot exceed 5000 characters.", nameof(exampleCode));

        ExampleCode = exampleCode?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets enforcement status.
    /// </summary>
    public void SetEnforced(bool isEnforced)
    {
        IsEnforced = isEnforced;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the tags.
    /// </summary>
    public void SetTags(string? tags)
    {
        if (!string.IsNullOrWhiteSpace(tags) && tags.Length > 200)
            throw new ArgumentException("Tags cannot exceed 200 characters.", nameof(tags));

        Tags = tags?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Activates the rule.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the rule.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Checks if this rule is more restrictive than another rule.
    /// </summary>
    public bool IsMoreRestrictiveThan(EngineeringRule other)
    {
        return Constraint.IsMoreRestrictiveThan(other.Constraint);
    }

    /// <summary>
    /// Checks if this rule conflicts with another rule.
    /// Rules conflict if they have the same name or overlapping concerns but different constraints.
    /// </summary>
    public bool ConflictsWith(EngineeringRule other)
    {
        // Basic conflict detection - can be enhanced with more sophisticated logic
        return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) 
            && !Constraint.Equals(other.Constraint);
    }
}
