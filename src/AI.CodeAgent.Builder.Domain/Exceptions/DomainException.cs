namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-specific exceptions.
/// Use this to distinguish domain logic errors from infrastructure or validation errors.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
