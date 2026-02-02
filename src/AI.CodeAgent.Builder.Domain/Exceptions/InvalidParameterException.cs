namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when parameter validation fails.
/// Examples: invalid version format, out-of-range value, wrong data type.
/// </summary>
public sealed class InvalidParameterException : DomainException
{
    public InvalidParameterException(string parameterName, string message)
        : base($"Parameter '{parameterName}' is invalid: {message}")
    {
        ParameterName = parameterName;
    }

    public InvalidParameterException(string parameterName, string providedValue, string expectedFormat)
        : base($"Parameter '{parameterName}' has invalid value '{providedValue}'. Expected format: {expectedFormat}")
    {
        ParameterName = parameterName;
        ProvidedValue = providedValue;
        ExpectedFormat = expectedFormat;
    }

    public string ParameterName { get; }
    public string? ProvidedValue { get; }
    public string? ExpectedFormat { get; }
}
