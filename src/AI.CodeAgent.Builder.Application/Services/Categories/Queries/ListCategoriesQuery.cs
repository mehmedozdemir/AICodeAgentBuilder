namespace AI.CodeAgent.Builder.Application.Services.Categories.Queries;

/// <summary>
/// Query to retrieve all active categories.
/// </summary>
public sealed class ListCategoriesQuery
{
    public bool IncludeInactive { get; }

    private ListCategoriesQuery(bool includeInactive)
    {
        IncludeInactive = includeInactive;
    }

    public static ListCategoriesQuery Create(bool includeInactive = false)
    {
        return new ListCategoriesQuery(includeInactive);
    }
}
