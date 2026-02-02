namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;

/// <summary>
/// Command to add an architecture pattern to a project profile.
/// </summary>
public sealed class AddArchitecturePatternCommand
{
    public Guid ProfileId { get; }
    public Guid PatternId { get; }

    private AddArchitecturePatternCommand(Guid profileId, Guid patternId)
    {
        ProfileId = profileId;
        PatternId = patternId;
    }

    public static AddArchitecturePatternCommand Create(Guid profileId, Guid patternId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        if (patternId == Guid.Empty)
            throw new ArgumentException("Pattern ID is required.", nameof(patternId));

        return new AddArchitecturePatternCommand(profileId, patternId);
    }
}
