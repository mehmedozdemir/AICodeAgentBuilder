namespace AI.CodeAgent.Builder.Application.DTOs;

/// <summary>
/// DTO for project metadata (Step 1 of wizard).
/// </summary>
public class ProjectMetadataDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetPlatform { get; set; } = string.Empty;
}

/// <summary>
/// DTO for complete project profile.
/// </summary>
public class ProjectProfileDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetPlatform { get; set; } = string.Empty;
    public string ArchitecturePattern { get; set; } = string.Empty;
    public Guid[] SelectedStackIds { get; set; } = Array.Empty<Guid>();
    public Guid[] SelectedRuleIds { get; set; } = Array.Empty<Guid>();
    public Dictionary<string, string> StackParameters { get; set; } = new();
}

/// <summary>
/// Result of artifact generation.
/// </summary>
public class GenerationResult
{
    public bool Success { get; set; }
    public List<string> GeneratedFiles { get; set; } = new();
    public string OutputDirectory { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
