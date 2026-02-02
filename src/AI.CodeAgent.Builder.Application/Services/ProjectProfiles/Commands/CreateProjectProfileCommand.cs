namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;

/// <summary>
/// Command to create a new project profile.
/// </summary>
public sealed class CreateProjectProfileCommand
{
    public string Name { get; }
    public string Description { get; }

    private CreateProjectProfileCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static CreateProjectProfileCommand Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Profile name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Profile description is required.", nameof(description));

        if (name.Length > 200)
            throw new ArgumentException("Profile name cannot exceed 200 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Profile description cannot exceed 1000 characters.", nameof(description));

        return new CreateProjectProfileCommand(name.Trim(), description.Trim());
    }
}
