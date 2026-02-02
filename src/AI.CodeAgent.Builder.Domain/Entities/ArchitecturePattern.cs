using AI.CodeAgent.Builder.Domain.Common;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents an architectural pattern or style.
/// Examples: Monolith, Modular Monolith, Microservices, Event-Driven, CQRS, Hexagonal.
/// 
/// ArchitecturePattern is an aggregate root that influences how generated
/// AI instructions are structured and what rules are emphasized.
/// </summary>
public sealed class ArchitecturePattern : BaseEntity
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _guidelines = string.Empty;

    // Private constructor for EF Core
    private ArchitecturePattern()
    {
    }

    private ArchitecturePattern(string name, string description, string guidelines)
    {
        SetName(name);
        SetDescription(description);
        SetGuidelines(guidelines);
        IsActive = true;
    }

    /// <summary>
    /// The name of the architecture pattern.
    /// Must be unique within the system.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// A detailed description of the architecture pattern.
    /// Explains when and why to use this pattern.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// Guidelines and best practices for implementing this architecture.
    /// Used in generating copilot instructions and code agent rules.
    /// </summary>
    public string Guidelines
    {
        get => _guidelines;
        private set => _guidelines = value;
    }

    /// <summary>
    /// Indicates the complexity level of this architecture.
    /// 1 = Simple (Monolith), 5 = Complex (Distributed Microservices).
    /// </summary>
    public int ComplexityLevel { get; private set; }

    /// <summary>
    /// Indicates whether this pattern is suitable for small teams (< 5 developers).
    /// </summary>
    public bool SuitableForSmallTeams { get; private set; }

    /// <summary>
    /// Indicates whether this pattern is suitable for large-scale systems.
    /// </summary>
    public bool SuitableForLargeScale { get; private set; }

    /// <summary>
    /// Key principles associated with this architecture (comma-separated).
    /// Examples: "separation of concerns", "bounded contexts", "event sourcing".
    /// </summary>
    public string? KeyPrinciples { get; private set; }

    /// <summary>
    /// Common anti-patterns to avoid when using this architecture (comma-separated).
    /// Used in generating code agent warnings and rules.
    /// </summary>
    public string? AntiPatterns { get; private set; }

    /// <summary>
    /// Indicates whether this architecture pattern is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates whether this pattern was created by AI.
    /// </summary>
    public bool IsAIGenerated { get; private set; }

    /// <summary>
    /// Factory method to create a new architecture pattern.
    /// </summary>
    public static ArchitecturePattern Create(
        string name,
        string description,
        string guidelines,
        int complexityLevel = 3,
        bool isAIGenerated = false)
    {
        var pattern = new ArchitecturePattern(name, description, guidelines)
        {
            IsAIGenerated = isAIGenerated
        };

        pattern.SetComplexityLevel(complexityLevel);
        return pattern;
    }

    /// <summary>
    /// Updates the architecture pattern name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Architecture pattern name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Architecture pattern name cannot exceed 100 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the architecture pattern description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Architecture pattern description cannot be empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Architecture pattern description cannot exceed 1000 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the architecture guidelines.
    /// </summary>
    public void SetGuidelines(string guidelines)
    {
        if (string.IsNullOrWhiteSpace(guidelines))
            throw new ArgumentException("Architecture guidelines cannot be empty.", nameof(guidelines));

        if (guidelines.Length > 2000)
            throw new ArgumentException("Architecture guidelines cannot exceed 2000 characters.", nameof(guidelines));

        _guidelines = guidelines.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the complexity level (1-5).
    /// </summary>
    public void SetComplexityLevel(int level)
    {
        if (level < 1 || level > 5)
            throw new ArgumentException("Complexity level must be between 1 and 5.", nameof(level));

        ComplexityLevel = level;
        SetUpdatedAt();
    }

    /// <summary>
    /// Configures team size suitability.
    /// </summary>
    public void SetTeamSizeSuitability(bool suitableForSmallTeams, bool suitableForLargeScale)
    {
        SuitableForSmallTeams = suitableForSmallTeams;
        SuitableForLargeScale = suitableForLargeScale;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets key principles for this architecture.
    /// </summary>
    public void SetKeyPrinciples(string? principles)
    {
        if (!string.IsNullOrWhiteSpace(principles) && principles.Length > 500)
            throw new ArgumentException("Key principles cannot exceed 500 characters.", nameof(principles));

        KeyPrinciples = principles?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets anti-patterns to avoid.
    /// </summary>
    public void SetAntiPatterns(string? antiPatterns)
    {
        if (!string.IsNullOrWhiteSpace(antiPatterns) && antiPatterns.Length > 500)
            throw new ArgumentException("Anti-patterns cannot exceed 500 characters.", nameof(antiPatterns));

        AntiPatterns = antiPatterns?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Activates the architecture pattern.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the architecture pattern.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Checks if this architecture is suitable for a given team size.
    /// </summary>
    public bool IsSuitableForTeamSize(int teamSize)
    {
        return teamSize switch
        {
            <= 5 => SuitableForSmallTeams,
            > 20 => SuitableForLargeScale,
            _ => true // Medium teams can use most architectures
        };
    }
}
