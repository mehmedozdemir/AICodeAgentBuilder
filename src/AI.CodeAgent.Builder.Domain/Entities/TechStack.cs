using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents a real-world technology stack within a category.
/// Examples: ASP.NET Core, Spring Boot, React, PostgreSQL, Docker.
/// 
/// TechStack is an aggregate root that owns its StackParameters.
/// It enforces invariants and maintains consistency within its boundary.
/// </summary>
public sealed class TechStack : BaseEntity
{
    private readonly List<StackParameter> _parameters = new();
    private string _name = string.Empty;
    private string _description = string.Empty;
    private TechStackVersion? _defaultVersion;

    // Private constructor for EF Core
    private TechStack()
    {
    }

    private TechStack(Guid categoryId, string name, string description)
    {
        CategoryId = categoryId;
        SetName(name);
        SetDescription(description);
        IsActive = true;
    }

    /// <summary>
    /// The category this tech stack belongs to.
    /// Each tech stack must belong to exactly one category.
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// The name of the technology stack.
    /// Must be unique within its category.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// Detailed description of the tech stack, its purpose, and use cases.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// Default version for this tech stack.
    /// Can be overridden at the project level.
    /// </summary>
    public TechStackVersion? DefaultVersion
    {
        get => _defaultVersion;
        private set => _defaultVersion = value;
    }

    /// <summary>
    /// Official documentation URL for this technology.
    /// </summary>
    public string? DocumentationUrl { get; private set; }

    /// <summary>
    /// Indicates whether this tech stack is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates whether this tech stack was created by AI.
    /// </summary>
    public bool IsAIGenerated { get; private set; }

    /// <summary>
    /// Tags for search and filtering (comma-separated).
    /// Examples: "web", "api", "cloud-native".
    /// </summary>
    public string? Tags { get; private set; }

    /// <summary>
    /// Read-only collection of parameters for this tech stack.
    /// Encapsulates the internal collection to prevent external modification.
    /// </summary>
    public IReadOnlyCollection<StackParameter> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// Factory method to create a new tech stack.
    /// </summary>
    public static TechStack Create(
        Guid categoryId,
        string name,
        string description,
        bool isAIGenerated = false)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));

        var techStack = new TechStack(categoryId, name, description)
        {
            IsAIGenerated = isAIGenerated
        };

        return techStack;
    }

    /// <summary>
    /// Updates the tech stack name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tech stack name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Tech stack name cannot exceed 100 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the tech stack description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Tech stack description cannot be empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Tech stack description cannot exceed 1000 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the default version for this tech stack.
    /// </summary>
    public void SetDefaultVersion(string version)
    {
        _defaultVersion = TechStackVersion.Create(version);
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the documentation URL.
    /// </summary>
    public void SetDocumentationUrl(string? url)
    {
        if (!string.IsNullOrWhiteSpace(url) && !Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid URL format.", nameof(url));

        DocumentationUrl = url?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the tags for search and filtering.
    /// </summary>
    public void SetTags(string? tags)
    {
        Tags = tags?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Adds a parameter to this tech stack.
    /// Ensures no duplicate parameter names exist.
    /// </summary>
    public void AddParameter(StackParameter parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));

        if (_parameters.Any(p => p.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A parameter with the name '{parameter.Name}' already exists.");

        _parameters.Add(parameter);
        SetUpdatedAt();
    }

    /// <summary>
    /// Removes a parameter from this tech stack.
    /// </summary>
    public void RemoveParameter(Guid parameterId)
    {
        var parameter = _parameters.FirstOrDefault(p => p.Id == parameterId);
        if (parameter == null)
            throw new InvalidOperationException($"Parameter with ID '{parameterId}' not found.");

        _parameters.Remove(parameter);
        SetUpdatedAt();
    }

    /// <summary>
    /// Gets a parameter by name.
    /// </summary>
    public StackParameter? GetParameter(string parameterName)
    {
        return _parameters.FirstOrDefault(p => 
            p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Activates the tech stack.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the tech stack.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Changes the category this tech stack belongs to.
    /// Use with caution as it may affect existing project configurations.
    /// </summary>
    public void ChangeCategory(Guid newCategoryId)
    {
        if (newCategoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty.", nameof(newCategoryId));

        CategoryId = newCategoryId;
        SetUpdatedAt();
    }
}
