using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Commands;

/// <summary>
/// Command to add a parameter to a tech stack.
/// </summary>
public sealed class AddStackParameterCommand
{
    public Guid TechStackId { get; }
    public string ParameterName { get; }
    public string Description { get; }
    public ParameterType ParameterType { get; }
    public bool IsRequired { get; }
    public string? DefaultValue { get; }
    public List<string> AllowedValues { get; }

    private AddStackParameterCommand(
        Guid techStackId,
        string parameterName,
        string description,
        ParameterType parameterType,
        bool isRequired,
        string? defaultValue,
        List<string> allowedValues)
    {
        TechStackId = techStackId;
        ParameterName = parameterName;
        Description = description;
        ParameterType = parameterType;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        AllowedValues = allowedValues;
    }

    public static AddStackParameterCommand Create(
        Guid techStackId,
        string parameterName,
        string description,
        ParameterType parameterType,
        bool isRequired = false,
        string? defaultValue = null,
        List<string>? allowedValues = null)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("Parameter name is required.", nameof(parameterName));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Parameter description is required.", nameof(description));

        if (parameterName.Length > 100)
            throw new ArgumentException("Parameter name cannot exceed 100 characters.", nameof(parameterName));

        if (description.Length > 500)
            throw new ArgumentException("Parameter description cannot exceed 500 characters.", nameof(description));

        return new AddStackParameterCommand(
            techStackId,
            parameterName.Trim(),
            description.Trim(),
            parameterType,
            isRequired,
            defaultValue?.Trim(),
            allowedValues ?? new List<string>());
    }
}
