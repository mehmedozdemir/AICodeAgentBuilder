namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Commands;

/// <summary>
/// Command to delete a tech stack.
/// </summary>
public sealed class DeleteTechStackCommand
{
    public Guid TechStackId { get; }

    private DeleteTechStackCommand(Guid techStackId)
    {
        TechStackId = techStackId;
    }

    public static DeleteTechStackCommand Create(Guid techStackId)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        return new DeleteTechStackCommand(techStackId);
    }
}
