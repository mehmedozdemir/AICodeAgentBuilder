using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Models;

/// <summary>
/// Data transfer object for ProjectProfile entity.
/// </summary>
public sealed class ProjectProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public int? TargetTeamSize { get; set; }
    public List<ProfileTechStackDto> TechStacks { get; set; } = new();
    public List<Guid> ArchitecturePatternIds { get; set; } = new();
    public List<Guid> EngineeringRuleIds { get; set; } = new();
    public bool IsValid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for tech stack selection with parameter values.
/// </summary>
public sealed class ProfileTechStackDto
{
    public Guid TechStackId { get; set; }
    public string TechStackName { get; set; } = string.Empty;
    public Dictionary<string, string> ParameterValues { get; set; } = new();
}
