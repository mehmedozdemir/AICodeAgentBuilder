// This file documents the Application Service interfaces required by the UI layer.
// Create these files in AI.CodeAgent.Builder.Application/Services/

namespace AI.CodeAgent.Builder.Application.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Application.DTOs;

/// <summary>
/// Service for managing technology categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets all categories ordered by DisplayOrder.
    /// </summary>
    Task<IEnumerable<Category>> GetAllCategoriesAsync();

    /// <summary>
    /// Gets a category by its ID.
    /// </summary>
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);

    /// <summary>
    /// Creates a new category.
    /// </summary>
    Task<Category> CreateCategoryAsync(Category category);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    Task UpdateCategoryAsync(Category category);

    /// <summary>
    /// Deletes a category and all associated tech stacks.
    /// </summary>
    Task DeleteCategoryAsync(Guid categoryId);
}

/// <summary>
/// Service for managing technology stacks.
/// </summary>
public interface ITechStackService
{
    /// <summary>
    /// Gets all tech stacks for a specific category.
    /// </summary>
    Task<IEnumerable<TechStack>> GetStacksByCategoryAsync(Guid categoryId);

    /// <summary>
    /// Gets a tech stack by its ID.
    /// </summary>
    Task<TechStack?> GetStackByIdAsync(Guid stackId);

    /// <summary>
    /// Creates a new tech stack.
    /// </summary>
    Task<TechStack> CreateStackAsync(TechStack stack);

    /// <summary>
    /// Updates an existing tech stack.
    /// </summary>
    Task UpdateStackAsync(TechStack stack);

    /// <summary>
    /// Deletes a tech stack.
    /// </summary>
    Task DeleteStackAsync(Guid stackId);
}

/// <summary>
/// Service for AI-driven generation operations.
/// </summary>
public interface IAIGenerationService
{
    /// <summary>
    /// Uses AI to research and populate categories and tech stacks.
    /// </summary>
    Task RefreshCategoriesAndStacksAsync();

    /// <summary>
    /// Gets all AI response history.
    /// </summary>
    Task<IEnumerable<AIResponse>> GetAIHistoryAsync();

    /// <summary>
    /// Gets AI response history filtered by status.
    /// </summary>
    Task<IEnumerable<AIResponse>> GetAIHistoryByStatusAsync(AIResponseStatus status);
}

/// <summary>
/// Service for managing architecture patterns.
/// </summary>
public interface IArchitectureService
{
    /// <summary>
    /// Gets all architecture patterns.
    /// </summary>
    Task<IEnumerable<ArchitecturePattern>> GetAllPatternsAsync();

    /// <summary>
    /// Gets an architecture pattern by ID.
    /// </summary>
    Task<ArchitecturePattern?> GetPatternByIdAsync(Guid patternId);
}

/// <summary>
/// Service for managing engineering rules.
/// </summary>
public interface IEngineeringRuleService
{
    /// <summary>
    /// Gets all engineering rules.
    /// </summary>
    Task<IEnumerable<EngineeringRule>> GetAllRulesAsync();

    /// <summary>
    /// Gets an engineering rule by ID.
    /// </summary>
    Task<EngineeringRule?> GetRuleByIdAsync(Guid ruleId);
}

/// <summary>
/// Service for generating AI configuration artifacts.
/// </summary>
public interface IArtifactGenerationService
{
    /// <summary>
    /// Generates copilot-instructions, configs, and templates based on project profile.
    /// </summary>
    Task<GenerationResult> GenerateArtifactsAsync(ProjectProfileDto projectProfile);

    /// <summary>
    /// Previews what would be generated without writing files.
    /// </summary>
    Task<string> PreviewGenerationAsync(ProjectProfileDto projectProfile);
}

// ========================================
// DTOs - Create these in Application/DTOs/
// ========================================

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

// ========================================
// Domain Updates Required
// ========================================

/*
 * Update AI.CodeAgent.Builder.Domain.Entities.StackParameter:
 * 
 * Add or ensure these properties exist:
 * - string ParameterName (or rename existing Name property)
 * - string DisplayName (user-friendly name)
 * - string? Description (help text)
 * - bool IsRequired (validation flag)
 * - string? DefaultValue (optional)
 * - string ParameterType (e.g., "string", "int", "bool", "enum")
 * 
 * These are needed by StackParameterViewModel for dynamic form generation.
 */

/*
 * Add AIResponseStatus.All enum value:
 * 
 * Add to AI.CodeAgent.Builder.Domain.Enums.AIResponseStatus:
 * - All = 0 (used for filtering "show all" in UI)
 * 
 * OR: Handle "All" in the ViewModel without domain enum change.
 */
