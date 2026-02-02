namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Commands;

/// <summary>
/// Command to update an existing tech stack.
/// </summary>
public sealed class UpdateTechStackCommand
{
    public Guid TechStackId { get; }
    public string Name { get; }
    public string Description { get; }
    public string? DefaultVersion { get; }
    public string? DocumentationUrl { get; }

    private UpdateTechStackCommand(
        Guid techStackId,
        string name,
        string description,
        string? defaultVersion,
        string? documentationUrl)
    {
        TechStackId = techStackId;
        Name = name;
        Description = description;
        DefaultVersion = defaultVersion;
        DocumentationUrl = documentationUrl;
    }

    public static UpdateTechStackCommand Create(
        Guid techStackId,
        string name,
        string description,
        string? defaultVersion = null,
        string? documentationUrl = null)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID is required.", nameof(techStackId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tech stack name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Tech stack description is required.", nameof(description));

        if (name.Length > 200)
            throw new ArgumentException("Tech stack name cannot exceed 200 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Tech stack description cannot exceed 1000 characters.", nameof(description));

        return new UpdateTechStackCommand(
            techStackId,
            name.Trim(),
            description.Trim(),
            defaultVersion?.Trim(),
            documentationUrl?.Trim());
    }
}
