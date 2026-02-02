namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Queries;

/// <summary>
/// Query to list tech stacks by category.
/// </summary>
public sealed class ListTechStacksByCategoryQuery
{
    public Guid CategoryId { get; }

    private ListTechStacksByCategoryQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public static ListTechStacksByCategoryQuery Create(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        return new ListTechStacksByCategoryQuery(categoryId);
    }
}
