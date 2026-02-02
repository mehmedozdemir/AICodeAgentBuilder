using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Commands;

/// <summary>
/// Command to create a new tech stack.
/// </summary>
public sealed class CreateTechStackCommand
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Description { get; }
    public string? DefaultVersion { get; }
    public string? DocumentationUrl { get; }

    private CreateTechStackCommand(
        Guid categoryId,
        string name,
        string description,
        string? defaultVersion,
        string? documentationUrl)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        DefaultVersion = defaultVersion;
        DocumentationUrl = documentationUrl;
    }

    public static CreateTechStackCommand Create(
        Guid categoryId,
        string name,
        string description,
        string? defaultVersion = null,
        string? documentationUrl = null)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID is required.", nameof(categoryId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tech stack name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Tech stack description is required.", nameof(description));

        if (name.Length > 200)
            throw new ArgumentException("Tech stack name cannot exceed 200 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Tech stack description cannot exceed 1000 characters.", nameof(description));

        return new CreateTechStackCommand(
            categoryId,
            name.Trim(),
            description.Trim(),
            defaultVersion?.Trim(),
            documentationUrl?.Trim());
    }
}
