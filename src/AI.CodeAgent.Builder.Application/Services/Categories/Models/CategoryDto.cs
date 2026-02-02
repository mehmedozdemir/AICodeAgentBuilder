namespace AI.CodeAgent.Builder.Application.Services.Categories.Models;

/// <summary>
/// Data transfer object for Category entity.
/// Used for returning category data to the UI layer.
/// </summary>
public sealed class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAIGenerated { get; set; }
    public int TechStackCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
