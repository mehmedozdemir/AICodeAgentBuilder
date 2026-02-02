namespace AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Commands;

/// <summary>
/// Command to generate all engineering artifacts (policies, templates, guidelines).
/// </summary>
public sealed class GenerateEngineeringArtifactsCommand
{
    public Guid ProfileId { get; }

    private GenerateEngineeringArtifactsCommand(Guid profileId)
    {
        ProfileId = profileId;
    }

    public static GenerateEngineeringArtifactsCommand Create(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        return new GenerateEngineeringArtifactsCommand(profileId);
    }
}
