namespace AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Commands;

/// <summary>
/// Command to generate Copilot instructions file from a project profile.
/// </summary>
public sealed class GenerateCopilotInstructionsCommand
{
    public Guid ProfileId { get; }

    private GenerateCopilotInstructionsCommand(Guid profileId)
    {
        ProfileId = profileId;
    }

    public static GenerateCopilotInstructionsCommand Create(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        return new GenerateCopilotInstructionsCommand(profileId);
    }
}
