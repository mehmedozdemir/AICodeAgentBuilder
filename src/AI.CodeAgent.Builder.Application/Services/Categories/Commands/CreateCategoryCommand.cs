namespace AI.CodeAgent.Builder.Application.Services.Categories.Commands;

/// <summary>
/// Command to create a new category.
/// </summary>
public sealed class CreateCategoryCommand
{
    public string Name { get; }
    public string Description { get; }
    public bool IsAIGenerated { get; }

    private CreateCategoryCommand(string name, string description, bool isAIGenerated)
    {
        Name = name;
        Description = description;
        IsAIGenerated = isAIGenerated;
    }

    public static CreateCategoryCommand Create(string name, string description, bool isAIGenerated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description is required.", nameof(description));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));

        return new CreateCategoryCommand(name.Trim(), description.Trim(), isAIGenerated);
    }
}
