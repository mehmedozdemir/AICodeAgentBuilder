namespace AI.CodeAgent.Builder.Application.Services.AIGeneration.Models;

/// <summary>
/// Result of AI-generated category data.
/// </summary>
public sealed class GeneratedCategoryResult
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Result of AI-generated tech stack data.
/// </summary>
public sealed class GeneratedTechStackResult
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DefaultVersion { get; set; }
    public string? DocumentationUrl { get; set; }
    public List<GeneratedParameterResult> Parameters { get; set; } = new();
}

/// <summary>
/// Result of AI-generated parameter data.
/// </summary>
public sealed class GeneratedParameterResult
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ParameterType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public List<string> AllowedValues { get; set; } = new();
}
