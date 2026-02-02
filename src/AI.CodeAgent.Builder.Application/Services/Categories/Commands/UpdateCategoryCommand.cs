namespace AI.CodeAgent.Builder.Application.Services.Categories.Commands;

/// <summary>
/// Command to update an existing category.
/// </summary>
public sealed class UpdateCategoryCommand
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Description { get; }

    private UpdateCategoryCommand(Guid categoryId, string name, string description)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
    }

    public static UpdateCategoryCommand Create(Guid categoryId, string name, string description)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description is required.", nameof(description));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));

        return new UpdateCategoryCommand(categoryId, name.Trim(), description.Trim());
    }
}
