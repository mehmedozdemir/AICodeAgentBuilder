namespace AI.CodeAgent.Builder.Application.Services.AIGeneration.Commands;

/// <summary>
/// Command to generate categories from AI.
/// </summary>
public sealed class GenerateCategoriesCommand
{
    public int Count { get; }
    public string? ContextHint { get; }

    private GenerateCategoriesCommand(int count, string? contextHint)
    {
        Count = count;
        ContextHint = contextHint;
    }

    public static GenerateCategoriesCommand Create(int count = 10, string? contextHint = null)
    {
        if (count <= 0 || count > 50)
            throw new ArgumentException("Count must be between 1 and 50.", nameof(count));

        return new GenerateCategoriesCommand(count, contextHint?.Trim());
    }
}
