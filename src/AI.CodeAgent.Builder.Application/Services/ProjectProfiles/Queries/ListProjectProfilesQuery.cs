namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Queries;

/// <summary>
/// Query to list all project profiles.
/// </summary>
public sealed class ListProjectProfilesQuery
{
    private ListProjectProfilesQuery()
    {
    }

    public static ListProjectProfilesQuery Create()
    {
        return new ListProjectProfilesQuery();
    }
}
