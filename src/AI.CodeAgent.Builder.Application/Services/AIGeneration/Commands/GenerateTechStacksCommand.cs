namespace AI.CodeAgent.Builder.Application.Services.AIGeneration.Commands;

/// <summary>
/// Command to generate tech stacks for a category from AI.
/// </summary>
public sealed class GenerateTechStacksCommand
{
    public Guid CategoryId { get; }
    public int Count { get; }

    private GenerateTechStacksCommand(Guid categoryId, int count)
    {
        CategoryId = categoryId;
        Count = count;
    }

    public static GenerateTechStacksCommand Create(Guid categoryId, int count = 10)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        if (count <= 0 || count > 30)
            throw new ArgumentException("Count must be between 1 and 30.", nameof(count));

        return new GenerateTechStacksCommand(categoryId, count);
    }
}
