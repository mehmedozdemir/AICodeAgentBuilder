namespace AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Commands;

/// <summary>
/// Command to generate AI agent configuration files from a project profile.
/// </summary>
public sealed class GenerateAIAgentConfigCommand
{
    public Guid ProfileId { get; }
    public string ConfigFormat { get; }

    private GenerateAIAgentConfigCommand(Guid profileId, string configFormat)
    {
        ProfileId = profileId;
        ConfigFormat = configFormat;
    }

    public static GenerateAIAgentConfigCommand Create(Guid profileId, string configFormat = "yaml")
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        var validFormats = new[] { "yaml", "json", "toml" };
        if (!validFormats.Contains(configFormat.ToLowerInvariant()))
            throw new ArgumentException($"Config format must be one of: {string.Join(", ", validFormats)}", nameof(configFormat));

        return new GenerateAIAgentConfigCommand(profileId, configFormat.ToLowerInvariant());
    }
}
