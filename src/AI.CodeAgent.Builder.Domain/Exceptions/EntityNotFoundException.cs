namespace AI.CodeAgent.Builder.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the repository.
/// </summary>
public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with id {id} was not found.")
    {
        EntityName = entityName;
        EntityId = id;
    }

    public string EntityName { get; }
    public Guid EntityId { get; }
}
