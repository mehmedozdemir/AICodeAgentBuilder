namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when an AI-generated response fails validation.
/// Used to distinguish AI validation failures from other domain validation issues.
/// </summary>
public sealed class AIValidationException : DomainException
{
    public AIValidationException(string message)
        : base(message)
    {
        ValidationErrors = new List<string> { message };
    }

    public AIValidationException(IEnumerable<string> validationErrors)
        : base("AI response validation failed.")
    {
        ValidationErrors = validationErrors.ToList();
    }

    public AIValidationException(string requestContext, IEnumerable<string> validationErrors)
        : base($"AI response validation failed for context '{requestContext}'.")
    {
        RequestContext = requestContext;
        ValidationErrors = validationErrors.ToList();
    }

    public string? RequestContext { get; }
    public IReadOnlyList<string> ValidationErrors { get; }
}
