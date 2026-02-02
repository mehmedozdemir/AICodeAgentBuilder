namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when a project profile configuration is invalid.
/// Examples: missing required tech stacks, conflicting rules, incomplete setup.
/// </summary>
public sealed class InvalidProfileConfigurationException : DomainException
{
    public InvalidProfileConfigurationException(string message)
        : base(message)
    {
    }

    public InvalidProfileConfigurationException(string profileName, string reason)
        : base($"Project profile '{profileName}' has invalid configuration: {reason}")
    {
        ProfileName = profileName;
        Reason = reason;
    }

    public string? ProfileName { get; }
    public string? Reason { get; }
}
