using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Services.TechStacks.Models;

/// <summary>
/// Data transfer object for TechStack entity.
/// </summary>
public sealed class TechStackDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DefaultVersion { get; set; }
    public string? DocumentationUrl { get; set; }
    public int PopularityScore { get; set; }
    public List<StackParameterDto> Parameters { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for StackParameter entity.
/// </summary>
public sealed class StackParameterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType ParameterType { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public List<string> AllowedValues { get; set; } = new();
}
