namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when a duplicate entity is detected where uniqueness is required.
/// Examples: duplicate category name, duplicate tech stack within a category.
/// </summary>
public sealed class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityType, string identifier)
        : base($"A {entityType} with identifier '{identifier}' already exists.")
    {
        EntityType = entityType;
        Identifier = identifier;
    }

    public string EntityType { get; }
    public string Identifier { get; }
}
