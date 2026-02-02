namespace AI.CodeAgent.Builder.Application.Services.Categories.Commands;

/// <summary>
/// Command to delete (deactivate) a category.
/// Categories are soft-deleted to maintain referential integrity.
/// </summary>
public sealed class DeleteCategoryCommand
{
    public Guid CategoryId { get; }

    private DeleteCategoryCommand(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public static DeleteCategoryCommand Create(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        return new DeleteCategoryCommand(categoryId);
    }
}
