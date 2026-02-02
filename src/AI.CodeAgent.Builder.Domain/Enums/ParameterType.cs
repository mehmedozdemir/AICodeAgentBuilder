namespace AI.CodeAgent.Builder.Domain.Enums;

/// <summary>
/// Defines the data type for a stack parameter.
/// Used to enforce type safety and enable proper UI rendering and validation.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// Free-text string value.
    /// Examples: project name, description, custom path.
    /// </summary>
    String = 1,

    /// <summary>
    /// Numeric value (integer or decimal).
    /// Examples: port number, thread count, timeout value.
    /// </summary>
    Number = 2,

    /// <summary>
    /// True/false value.
    /// Examples: enable feature, use HTTPS, strict mode.
    /// </summary>
    Boolean = 3,

    /// <summary>
    /// Predefined list of options.
    /// Examples: log level (Debug, Info, Warning, Error), environment (Dev, Staging, Prod).
    /// </summary>
    Enum = 4,

    /// <summary>
    /// Version string following semantic versioning.
    /// Examples: 8.0, 3.11.0, 2024.1.
    /// </summary>
    Version = 5
}
