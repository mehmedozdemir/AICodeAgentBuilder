using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Enums;
using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents a configurable parameter for a technology stack.
/// Examples: Version, Port, Enable Feature, Environment.
/// 
/// StackParameter is owned by TechStack and cannot exist independently.
/// It is not an aggregate root.
/// </summary>
public sealed class StackParameter : BaseEntity
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string? _defaultValue;
    private List<string> _allowedValues = new();

    // Private constructor for EF Core
    private StackParameter()
    {
    }

    private StackParameter(string name, string description, ParameterType parameterType)
    {
        SetName(name);
        SetDescription(description);
        ParameterType = parameterType;
        IsRequired = false;
    }

    /// <summary>
    /// The name of the parameter.
    /// Must be unique within the tech stack.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// Description explaining the purpose and usage of this parameter.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// The data type of this parameter.
    /// Determines validation rules and UI rendering.
    /// </summary>
    public ParameterType ParameterType { get; private set; }

    /// <summary>
    /// Default value for this parameter if not specified.
    /// Stored as string and converted based on ParameterType.
    /// </summary>
    public string? DefaultValue
    {
        get => _defaultValue;
        private set => _defaultValue = value;
    }

    /// <summary>
    /// Indicates whether this parameter must be provided.
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Allowed values for Enum-type parameters.
    /// Empty for non-enum types.
    /// </summary>
    public IReadOnlyCollection<string> AllowedValues => _allowedValues.AsReadOnly();

    /// <summary>
    /// Display order within the tech stack parameters.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Factory method to create a new stack parameter.
    /// </summary>
    public static StackParameter Create(
        string name,
        string description,
        ParameterType parameterType)
    {
        return new StackParameter(name, description, parameterType);
    }

    /// <summary>
    /// Updates the parameter name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Parameter name cannot exceed 100 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the parameter description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Parameter description cannot be empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Parameter description cannot exceed 500 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the default value for this parameter.
    /// Validates the value against the parameter type.
    /// </summary>
    public void SetDefaultValue(string? value)
    {
        if (value != null)
        {
            ValidateValue(value);
        }

        _defaultValue = value;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets whether this parameter is required.
    /// </summary>
    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets allowed values for enum-type parameters.
    /// Only applicable when ParameterType is Enum.
    /// </summary>
    public void SetAllowedValues(IEnumerable<string> values)
    {
        if (ParameterType != ParameterType.Enum)
            throw new InvalidOperationException("Allowed values can only be set for Enum parameters.");

        if (values == null || !values.Any())
            throw new ArgumentException("Enum parameters must have at least one allowed value.", nameof(values));

        _allowedValues = values.Select(v => v.Trim()).Distinct().ToList();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the display order.
    /// </summary>
    public void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Display order cannot be negative.", nameof(order));

        DisplayOrder = order;
        SetUpdatedAt();
    }

    /// <summary>
    /// Validates a value against this parameter's type and constraints.
    /// </summary>
    public void ValidateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Parameter value cannot be empty.", nameof(value));

        switch (ParameterType)
        {
            case ParameterType.Boolean:
                if (!bool.TryParse(value, out _))
                    throw new ArgumentException($"'{value}' is not a valid boolean value.");
                break;

            case ParameterType.Number:
                if (!decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException($"'{value}' is not a valid number.");
                break;

            case ParameterType.Enum:
                if (!_allowedValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"'{value}' is not in the list of allowed values.");
                break;

            case ParameterType.Version:
                _ = TechStackVersion.Create(value); // Will throw if invalid
                break;

            case ParameterType.String:
                // No additional validation needed
                break;
        }
    }

    /// <summary>
    /// Creates a ParameterValue from a string input.
    /// </summary>
    public ParameterValue CreateValue(string value)
    {
        ValidateValue(value);

        return ParameterType switch
        {
            ParameterType.String => ParameterValue.CreateString(value),
            ParameterType.Number => ParameterValue.CreateNumber(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture)),
            ParameterType.Boolean => ParameterValue.CreateBoolean(bool.Parse(value)),
            ParameterType.Enum => ParameterValue.CreateEnum(value),
            ParameterType.Version => ParameterValue.CreateVersion(value),
            _ => throw new InvalidOperationException($"Unsupported parameter type: {ParameterType}")
        };
    }
}
