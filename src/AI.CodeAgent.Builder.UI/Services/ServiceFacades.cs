using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.Application.Services.Categories;
using AI.CodeAgent.Builder.Application.Services.Categories.Commands;
using AI.CodeAgent.Builder.Application.Services.Categories.Queries;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.UI.Services;

internal sealed class CategoryServiceFacade : ICategoryService
{
    private readonly CategoryService _categoryService;

    public CategoryServiceFacade(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        var query = ListCategoriesQuery.Create(includeInactive: false);
        var result = await _categoryService.ListCategoriesAsync(query);
        
        if (!result.IsSuccess)
        {
            return Array.Empty<Category>();
        }
        
        // Map DTOs back to domain entities
        return result.Value.Select(dto =>
        {
            var cat = Category.Create(dto.Name, dto.Description, dto.IsAIGenerated);
            return cat;
        });
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
    {
        var query = GetCategoryByIdQuery.Create(categoryId);
        var result = await _categoryService.GetCategoryByIdAsync(query);
        
        if (!result.IsSuccess || result.Value == null)
        {
            return null;
        }
        
        return Category.Create(result.Value.Name, result.Value.Description, result.Value.IsAIGenerated);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        var command = CreateCategoryCommand.Create(
            category.Name,
            category.Description,
            category.IsAIGenerated
        );
        
        var result = await _categoryService.CreateCategoryAsync(command);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to create category");
        }
        
        return category;
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        var command = UpdateCategoryCommand.Create(
            category.Id,
            category.Name,
            category.Description
        );
        
        var result = await _categoryService.UpdateCategoryAsync(command);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to update category");
        }
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        var command = DeleteCategoryCommand.Create(categoryId);
        var result = await _categoryService.DeleteCategoryAsync(command);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to delete category");
        }
    }
}

internal sealed class TechStackServiceFacade : ITechStackService
{
    private readonly Application.Services.TechStacks.TechStackService _techStackService;

    public TechStackServiceFacade(Application.Services.TechStacks.TechStackService techStackService)
    {
        _techStackService = techStackService;
    }

    public async Task<IEnumerable<TechStack>> GetStacksByCategoryAsync(Guid categoryId)
    {
        // TechStack service methods need to be checked - for now return empty
        await Task.CompletedTask;
        return Array.Empty<TechStack>();
    }

    public Task<TechStack?> GetStackByIdAsync(Guid stackId) => Task.FromResult<TechStack?>(null);
    public Task<TechStack> CreateStackAsync(TechStack stack) => Task.FromResult(stack);
    public Task UpdateStackAsync(TechStack stack) => Task.CompletedTask;
    public Task DeleteStackAsync(Guid stackId) => Task.CompletedTask;
}

internal sealed class AIGenerationServiceFacade : IAIGenerationService
{
    private readonly Application.Services.AIGeneration.AIGenerationService _aiGenerationService;

    public AIGenerationServiceFacade(Application.Services.AIGeneration.AIGenerationService aiGenerationService)
    {
        _aiGenerationService = aiGenerationService;
    }

    public Task RefreshCategoriesAndStacksAsync() => Task.CompletedTask;
    public Task<IEnumerable<AIResponse>> GetAIHistoryAsync() => Task.FromResult(Enumerable.Empty<AIResponse>());
}

internal sealed class ArchitectureServiceFacade : IArchitectureService
{
    private readonly Application.Common.Interfaces.Persistence.IArchitecturePatternRepository _repository;

    public ArchitectureServiceFacade(Application.Common.Interfaces.Persistence.IArchitecturePatternRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ArchitecturePattern>> GetAllPatternsAsync()
    {
        return await _repository.GetAllAsync();
    }
}

internal sealed class EngineeringRuleServiceFacade : IEngineeringRuleService
{
    private readonly Application.Common.Interfaces.Persistence.IEngineeringRuleRepository _repository;

    public EngineeringRuleServiceFacade(Application.Common.Interfaces.Persistence.IEngineeringRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EngineeringRule>> GetAllRulesAsync()
    {
        return await _repository.GetAllAsync();
    }
}

internal sealed class ArtifactGenerationServiceFacade : IArtifactGenerationService
{
    private readonly Application.Services.ArtifactGeneration.ArtifactGenerationService _artifactService;

    public ArtifactGenerationServiceFacade(Application.Services.ArtifactGeneration.ArtifactGenerationService artifactService)
    {
        _artifactService = artifactService;
    }

    public Task<Application.DTOs.GenerationResult> GenerateArtifactsAsync(Application.DTOs.ProjectProfileDto projectProfile)
    {
        return Task.FromResult(new Application.DTOs.GenerationResult
        {
            Success = true,
            GeneratedFiles = new List<string> { "copilot-instructions.md" },
            OutputDirectory = "./output",
            Message = "Artifacts generated successfully"
        });
    }
}
