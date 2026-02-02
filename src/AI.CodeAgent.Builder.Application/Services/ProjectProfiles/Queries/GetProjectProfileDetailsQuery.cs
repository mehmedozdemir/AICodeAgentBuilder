namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Queries;

/// <summary>
/// Query to retrieve a project profile with all details.
/// </summary>
public sealed class GetProjectProfileDetailsQuery
{
    public Guid ProfileId { get; }

    private GetProjectProfileDetailsQuery(Guid profileId)
    {
        ProfileId = profileId;
    }

    public static GetProjectProfileDetailsQuery Create(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        return new GetProjectProfileDetailsQuery(profileId);
    }
}
