using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Domain.ValueObjects;

/// <summary>
/// Value object representing a strongly-typed parameter value.
/// Encapsulates the value and its type, ensuring type safety across the domain.
/// </summary>
public sealed class ParameterValue : ValueObject
{
    private ParameterValue(ParameterType type, string value)
    {
        Type = type;
        Value = value;
    }

    public ParameterType Type { get; }
    public string Value { get; }

    /// <summary>
    /// Creates a string parameter value.
    /// </summary>
    public static ParameterValue CreateString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("String value cannot be null or empty.", nameof(value));

        return new ParameterValue(ParameterType.String, value);
    }

    /// <summary>
    /// Creates a number parameter value.
    /// </summary>
    public static ParameterValue CreateNumber(decimal value)
    {
        return new ParameterValue(ParameterType.Number, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Creates a boolean parameter value.
    /// </summary>
    public static ParameterValue CreateBoolean(bool value)
    {
        return new ParameterValue(ParameterType.Boolean, value.ToString().ToLowerInvariant());
    }

    /// <summary>
    /// Creates an enum parameter value.
    /// </summary>
    public static ParameterValue CreateEnum(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Enum value cannot be null or empty.", nameof(value));

        return new ParameterValue(ParameterType.Enum, value);
    }

    /// <summary>
    /// Creates a version parameter value.
    /// </summary>
    public static ParameterValue CreateVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty.", nameof(version));

        // Basic version validation - can be enhanced with regex if needed
        if (!version.Contains('.') && !char.IsDigit(version[0]))
            throw new ArgumentException("Version must be in a valid format (e.g., 8.0, 3.11.0).", nameof(version));

        return new ParameterValue(ParameterType.Version, version);
    }

    /// <summary>
    /// Attempts to get the value as a boolean.
    /// </summary>
    public bool TryGetBoolean(out bool result)
    {
        result = false;
        if (Type != ParameterType.Boolean)
            return false;

        return bool.TryParse(Value, out result);
    }

    /// <summary>
    /// Attempts to get the value as a decimal number.
    /// </summary>
    public bool TryGetNumber(out decimal result)
    {
        result = 0;
        if (Type != ParameterType.Number)
            return false;

        return decimal.TryParse(Value, System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, out result);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return Value;
    }

    public override string ToString() => $"{Type}: {Value}";
}
