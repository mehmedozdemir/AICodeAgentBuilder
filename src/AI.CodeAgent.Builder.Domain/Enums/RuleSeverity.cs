namespace AI.CodeAgent.Builder.Domain.Enums;

/// <summary>
/// Defines the severity level of an engineering rule.
/// Determines how violations are treated in the generated code agent instructions.
/// </summary>
public enum RuleSeverity
{
    /// <summary>
    /// Informational guidance.
    /// Does not block code generation but provides recommendations.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning about potential issues.
    /// Should be addressed but does not prevent code generation.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error that must be resolved.
    /// Violations should be treated as critical issues in generated instructions.
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical requirement that must be enforced.
    /// Represents non-negotiable constraints.
    /// </summary>
    Critical = 4
}
