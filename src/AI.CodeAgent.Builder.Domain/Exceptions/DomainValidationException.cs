namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when domain validation rules are violated.
/// </summary>
public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message)
        : base(message)
    {
        ValidationErrors = new List<string> { message };
    }

    public DomainValidationException(IEnumerable<string> validationErrors)
        : base("One or more domain validation errors occurred.")
    {
        ValidationErrors = validationErrors.ToList();
    }

    public IReadOnlyList<string> ValidationErrors { get; }
}
