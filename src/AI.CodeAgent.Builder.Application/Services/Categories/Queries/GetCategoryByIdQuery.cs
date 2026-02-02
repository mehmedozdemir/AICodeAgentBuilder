namespace AI.CodeAgent.Builder.Application.Services.Categories.Queries;

/// <summary>
/// Query to retrieve a single category by ID.
/// </summary>
public sealed class GetCategoryByIdQuery
{
    public Guid CategoryId { get; }

    private GetCategoryByIdQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public static GetCategoryByIdQuery Create(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        return new GetCategoryByIdQuery(categoryId);
    }
}
