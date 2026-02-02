namespace AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Models;

/// <summary>
/// Represents a generated artifact file.
/// </summary>
public sealed class GeneratedArtifact
{
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Result of artifact generation containing multiple files.
/// </summary>
public sealed class ArtifactGenerationResult
{
    public List<GeneratedArtifact> Artifacts { get; set; } = new();
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime GeneratedAt { get; set; }
}
