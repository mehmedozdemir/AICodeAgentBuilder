namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;

/// <summary>
/// Command to update project profile metadata.
/// </summary>
public sealed class UpdateProjectProfileCommand
{
    public Guid ProfileId { get; }
    public string Name { get; }
    public string Description { get; }
    public string? ProjectName { get; }
    public int? TargetTeamSize { get; }

    private UpdateProjectProfileCommand(
        Guid profileId,
        string name,
        string description,
        string? projectName,
        int? targetTeamSize)
    {
        ProfileId = profileId;
        Name = name;
        Description = description;
        ProjectName = projectName;
        TargetTeamSize = targetTeamSize;
    }

    public static UpdateProjectProfileCommand Create(
        Guid profileId,
        string name,
        string description,
        string? projectName = null,
        int? targetTeamSize = null)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile ID is required.", nameof(profileId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Profile name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Profile description is required.", nameof(description));

        if (name.Length > 200)
            throw new ArgumentException("Profile name cannot exceed 200 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Profile description cannot exceed 1000 characters.", nameof(description));

        if (targetTeamSize.HasValue && (targetTeamSize.Value < 1 || targetTeamSize.Value > 1000))
            throw new ArgumentException("Target team size must be between 1 and 1000.", nameof(targetTeamSize));

        return new UpdateProjectProfileCommand(
            profileId,
            name.Trim(),
            description.Trim(),
            projectName?.Trim(),
            targetTeamSize);
    }
}
