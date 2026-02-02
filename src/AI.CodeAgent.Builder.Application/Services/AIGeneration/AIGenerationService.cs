using System.Text.Json;
using AI.CodeAgent.Builder.Application.Common;
using AI.CodeAgent.Builder.Application.Common.Interfaces.AI;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Services.AIGeneration.Commands;
using AI.CodeAgent.Builder.Application.Services.AIGeneration.Models;
using AI.CodeAgent.Builder.Application.Services.Categories.Models;
using AI.CodeAgent.Builder.Application.Services.TechStacks.Models;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Application.Services.AIGeneration;

/// <summary>
/// Application service for AI-driven data generation.
/// Orchestrates AI calls, validates responses, and persists generated data.
/// </summary>
public sealed class AIGenerationService
{
    private readonly IAIProvider _aiProvider;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITechStackRepository _techStackRepository;
    private readonly IAIResponseRepository _aiResponseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AIGenerationService(
        IAIProvider aiProvider,
        ICategoryRepository categoryRepository,
        ITechStackRepository techStackRepository,
        IAIResponseRepository aiResponseRepository,
        IUnitOfWork unitOfWork)
    {
        _aiProvider = aiProvider ?? throw new ArgumentNullException(nameof(aiProvider));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _techStackRepository = techStackRepository ?? throw new ArgumentNullException(nameof(techStackRepository));
        _aiResponseRepository = aiResponseRepository ?? throw new ArgumentNullException(nameof(aiResponseRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Generates categories from AI.
    /// Validates AI output and persists to database.
    /// </summary>
    public async Task<Result<IEnumerable<CategoryDto>>> GenerateCategoriesAsync(
        GenerateCategoriesCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Build prompt
            var prompt = BuildCategoryGenerationPrompt(command.Count, command.ContextHint);

            // Call AI provider
            var request = AIPromptRequest.Create(
                prompt,
                "GenerateCategories",
                systemMessage: "You are an expert in software engineering categorization and technology classification.",
                maxTokens: 2000,
                temperature: 0.7,
                expectedFormat: "JSON");

            var aiResponse = await _aiProvider.SendPromptAsync(request, cancellationToken);

            // Persist AI response for audit
            var aiResponseEntity = await PersistAIResponseAsync(
                request.Prompt,
                request.RequestContext,
                aiResponse,
                cancellationToken);

            if (!aiResponse.IsSuccess)
            {
                return Result<IEnumerable<CategoryDto>>.Failure($"AI generation failed: {aiResponse.ErrorMessage}");
            }

            // Parse and validate AI output
            var generatedCategories = ParseCategoriesFromJSON(aiResponse.Content);
            if (generatedCategories == null || !generatedCategories.Any())
            {
                await MarkAIResponseAsRejectedAsync(aiResponseEntity, "Failed to parse categories from AI response.", cancellationToken);
                return Result<IEnumerable<CategoryDto>>.Failure("AI response did not contain valid category data.");
            }

            // Validate and persist categories
            var createdCategories = new List<CategoryDto>();
            foreach (var genCategory in generatedCategories)
            {
                // Check for duplicates
                if (await _categoryRepository.ExistsByNameAsync(genCategory.Name, cancellationToken))
                {
                    continue; // Skip duplicates
                }

                // Create domain entity
                var category = Category.Create(genCategory.Name, genCategory.Description, isAIGenerated: true);
                await _categoryRepository.AddAsync(category, cancellationToken);

                createdCategories.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    IsAIGenerated = category.IsAIGenerated,
                    TechStackCount = 0,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt ?? category.CreatedAt
                });
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mark AI response as validated
            await MarkAIResponseAsValidatedAsync(aiResponseEntity, "System", cancellationToken);

            return Result<IEnumerable<CategoryDto>>.Success(createdCategories);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CategoryDto>>.Failure($"Failed to generate categories: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates tech stacks for a category from AI.
    /// </summary>
    public async Task<Result<IEnumerable<TechStackDto>>> GenerateTechStacksAsync(
        GenerateTechStacksCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<IEnumerable<TechStackDto>>.Failure($"Category with ID '{command.CategoryId}' not found.");
            }

            // Build prompt
            var prompt = BuildTechStackGenerationPrompt(category.Name, category.Description, command.Count);

            // Call AI provider
            var request = AIPromptRequest.Create(
                prompt,
                "GenerateTechStacks",
                systemMessage: "You are an expert in modern software development technologies and frameworks.",
                maxTokens: 3000,
                temperature: 0.6,
                expectedFormat: "JSON");

            var aiResponse = await _aiProvider.SendPromptAsync(request, cancellationToken);

            // Persist AI response
            var aiResponseEntity = await PersistAIResponseAsync(
                request.Prompt,
                request.RequestContext,
                aiResponse,
                cancellationToken);

            if (!aiResponse.IsSuccess)
            {
                return Result<IEnumerable<TechStackDto>>.Failure($"AI generation failed: {aiResponse.ErrorMessage}");
            }

            // Parse and validate
            var generatedStacks = ParseTechStacksFromJSON(aiResponse.Content);
            if (generatedStacks == null || !generatedStacks.Any())
            {
                await MarkAIResponseAsRejectedAsync(aiResponseEntity, "Failed to parse tech stacks from AI response.", cancellationToken);
                return Result<IEnumerable<TechStackDto>>.Failure("AI response did not contain valid tech stack data.");
            }

            // Persist tech stacks
            var createdStacks = new List<TechStackDto>();
            foreach (var genStack in generatedStacks)
            {
                // Check for duplicates
                if (await _techStackRepository.ExistsByNameAsync(command.CategoryId, genStack.Name, cancellationToken))
                {
                    continue;
                }

                // Create domain entity
                var techStack = TechStack.Create(command.CategoryId, genStack.Name, genStack.Description);

                if (!string.IsNullOrWhiteSpace(genStack.DefaultVersion))
                    techStack.SetDefaultVersion(genStack.DefaultVersion);

                if (!string.IsNullOrWhiteSpace(genStack.DocumentationUrl))
                    techStack.SetDocumentationUrl(genStack.DocumentationUrl);

                // Add parameters
                foreach (var genParam in genStack.Parameters)
                {
                    if (!Enum.TryParse<ParameterType>(genParam.ParameterType, true, out var paramType))
                        continue;

                    var parameter = StackParameter.Create(genParam.Name, genParam.Description, paramType);

                    if (genParam.IsRequired)
                        parameter.SetRequired(true);

                    if (!string.IsNullOrWhiteSpace(genParam.DefaultValue))
                        parameter.SetDefaultValue(genParam.DefaultValue);

                    if (genParam.AllowedValues.Any())
                        parameter.SetAllowedValues(genParam.AllowedValues.ToArray());

                    techStack.AddParameter(parameter);
                }

                await _techStackRepository.AddAsync(techStack, cancellationToken);

                createdStacks.Add(new TechStackDto
                {
                    Id = techStack.Id,
                    CategoryId = techStack.CategoryId,
                    CategoryName = category.Name,
                    Name = techStack.Name,
                    Description = techStack.Description,
                    DefaultVersion = techStack.DefaultVersion?.ToString(),
                    DocumentationUrl = techStack.DocumentationUrl,
                    Parameters = techStack.Parameters.Select(p => new StackParameterDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        ParameterType = p.ParameterType,
                        IsRequired = p.IsRequired,
                        DefaultValue = p.DefaultValue,
                        AllowedValues = p.AllowedValues.ToList()
                    }).ToList(),
                    CreatedAt = techStack.CreatedAt,
                    UpdatedAt = techStack.UpdatedAt ?? techStack.CreatedAt
                });
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mark AI response as validated
            await MarkAIResponseAsValidatedAsync(aiResponseEntity, "System", cancellationToken);

            return Result<IEnumerable<TechStackDto>>.Success(createdStacks);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TechStackDto>>.Failure($"Failed to generate tech stacks: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates parameters for an existing tech stack from AI.
    /// </summary>
    public async Task<Result<TechStackDto>> GenerateStackParametersAsync(
        GenerateStackParametersCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate tech stack exists
            var techStack = await _techStackRepository.GetWithParametersAsync(command.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result<TechStackDto>.Failure($"Tech stack with ID '{command.TechStackId}' not found.");
            }

            // Build prompt
            var prompt = BuildParameterGenerationPrompt(techStack.Name, techStack.Description);

            // Call AI provider
            var request = AIPromptRequest.Create(
                prompt,
                "GenerateStackParameters",
                systemMessage: "You are an expert in software configuration and parameterization.",
                maxTokens: 1500,
                temperature: 0.5,
                expectedFormat: "JSON");

            var aiResponse = await _aiProvider.SendPromptAsync(request, cancellationToken);

            // Persist AI response
            var aiResponseEntity = await PersistAIResponseAsync(
                request.Prompt,
                request.RequestContext,
                aiResponse,
                cancellationToken);

            if (!aiResponse.IsSuccess)
            {
                return Result<TechStackDto>.Failure($"AI generation failed: {aiResponse.ErrorMessage}");
            }

            // Parse and validate
            var generatedParams = ParseParametersFromJSON(aiResponse.Content);
            if (generatedParams == null || !generatedParams.Any())
            {
                await MarkAIResponseAsRejectedAsync(aiResponseEntity, "Failed to parse parameters from AI response.", cancellationToken);
                return Result<TechStackDto>.Failure("AI response did not contain valid parameter data.");
            }

            // Add parameters to tech stack
            foreach (var genParam in generatedParams)
            {
                // Skip if parameter already exists
                if (techStack.Parameters.Any(p => p.Name.Equals(genParam.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (!Enum.TryParse<ParameterType>(genParam.ParameterType, true, out var paramType))
                    continue;

                var parameter = StackParameter.Create(genParam.Name, genParam.Description, paramType);

                if (genParam.IsRequired)
                    parameter.SetRequired(true);

                if (!string.IsNullOrWhiteSpace(genParam.DefaultValue))
                    parameter.SetDefaultValue(genParam.DefaultValue);

                if (genParam.AllowedValues.Any())
                    parameter.SetAllowedValues(genParam.AllowedValues.ToArray());

                techStack.AddParameter(parameter);
            }

            await _techStackRepository.UpdateAsync(techStack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mark AI response as validated
            await MarkAIResponseAsValidatedAsync(aiResponseEntity, "System", cancellationToken);

            // Get category name
            var category = await _categoryRepository.GetByIdAsync(techStack.CategoryId, cancellationToken);
            var categoryName = category?.Name ?? "Unknown";

            var dto = new TechStackDto
            {
                Id = techStack.Id,
                CategoryId = techStack.CategoryId,
                CategoryName = categoryName,
                Name = techStack.Name,
                Description = techStack.Description,
                DefaultVersion = techStack.DefaultVersion?.ToString(),
                DocumentationUrl = techStack.DocumentationUrl,
                Parameters = techStack.Parameters.Select(p => new StackParameterDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ParameterType = p.ParameterType,
                    IsRequired = p.IsRequired,
                    DefaultValue = p.DefaultValue,
                    AllowedValues = p.AllowedValues.ToList()
                }).ToList(),
                CreatedAt = techStack.CreatedAt,
                UpdatedAt = techStack.UpdatedAt ?? techStack.CreatedAt
            };

            return Result<TechStackDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TechStackDto>.Failure($"Failed to generate parameters: {ex.Message}");
        }
    }

    #region Prompt Building

    private static string BuildCategoryGenerationPrompt(int count, string? contextHint)
    {
        var prompt = $@"Generate {count} high-level technology categories for software engineering.

Each category should represent a major area of software development (e.g., Backend, Frontend, Database, DevOps).

{(string.IsNullOrWhiteSpace(contextHint) ? "" : $"Context: {contextHint}\n")}
Return the result as a JSON array with this exact structure:
[
  {{
    ""Name"": ""Category name (max 100 chars)"",
    ""Description"": ""Detailed description (max 500 chars)""
  }}
]

Requirements:
- Use widely recognized categories
- Names must be unique and concise
- Descriptions should explain the category's scope
- Return valid JSON only, no additional text";

        return prompt;
    }

    private static string BuildTechStackGenerationPrompt(string categoryName, string categoryDescription, int count)
    {
        var prompt = $@"Generate {count} popular technology stacks for the category: {categoryName}

Category Description: {categoryDescription}

Return the result as a JSON array with this structure:
[
  {{
    ""Name"": ""Tech stack name"",
    ""Description"": ""Detailed description"",
    ""DefaultVersion"": ""Current stable version (optional)"",
    ""DocumentationUrl"": ""Official documentation URL (optional)"",
    ""Parameters"": [
      {{
        ""Name"": ""Parameter name"",
        ""Description"": ""Parameter description"",
        ""ParameterType"": ""String|Number|Boolean|Enum|Version"",
        ""IsRequired"": true/false,
        ""DefaultValue"": ""default value (optional)"",
        ""AllowedValues"": [""value1"", ""value2""] // for Enum type only
      }}
    ]
  }}
]

Requirements:
- Use real, widely-adopted technologies
- Include current version numbers
- Provide official documentation URLs
- Add 2-5 meaningful parameters per tech stack
- Return valid JSON only";

        return prompt;
    }

    private static string BuildParameterGenerationPrompt(string techStackName, string techStackDescription)
    {
        var prompt = $@"Generate configuration parameters for the technology: {techStackName}

Description: {techStackDescription}

Return the result as a JSON array:
[
  {{
    ""Name"": ""Parameter name"",
    ""Description"": ""What this parameter controls"",
    ""ParameterType"": ""String|Number|Boolean|Enum|Version"",
    ""IsRequired"": true/false,
    ""DefaultValue"": ""default value (optional)"",
    ""AllowedValues"": [""option1"", ""option2""] // for Enum type
  }}
]

Requirements:
- Generate 3-8 meaningful parameters
- Include common configuration options
- Use appropriate parameter types
- Provide sensible default values
- Return valid JSON only";

        return prompt;
    }

    #endregion

    #region Parsing

    private static List<GeneratedCategoryResult>? ParseCategoriesFromJSON(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<GeneratedCategoryResult>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    private static List<GeneratedTechStackResult>? ParseTechStacksFromJSON(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<GeneratedTechStackResult>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    private static List<GeneratedParameterResult>? ParseParametersFromJSON(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<GeneratedParameterResult>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region AI Response Management

    private async Task<AIResponse> PersistAIResponseAsync(
        string prompt,
        string context,
        AIPromptResponse aiResponse,
        CancellationToken cancellationToken)
    {
        var entity = AIResponse.Create(prompt, aiResponse.Content, context);

        if (aiResponse.PromptTokens.HasValue)
            entity.SetPerformanceMetrics(aiResponse.PromptTokens.Value + (aiResponse.CompletionTokens ?? 0), null);

        if (!aiResponse.IsSuccess && !string.IsNullOrWhiteSpace(aiResponse.ErrorMessage))
        {
            entity.MarkAsRejected(aiResponse.ErrorMessage, "AI Provider");
        }

        await _aiResponseRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    private async Task MarkAIResponseAsValidatedAsync(
        AIResponse aiResponse,
        string validatorName,
        CancellationToken cancellationToken)
    {
        aiResponse.MarkAsValidated(validatorName);
        await _aiResponseRepository.UpdateAsync(aiResponse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task MarkAIResponseAsRejectedAsync(
        AIResponse aiResponse,
        string reason,
        CancellationToken cancellationToken)
    {
        aiResponse.MarkAsRejected(reason, "System");
        await _aiResponseRepository.UpdateAsync(aiResponse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
