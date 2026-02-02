using AI.CodeAgent.Builder.Application.Common;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Services.TechStacks.Commands;
using AI.CodeAgent.Builder.Application.Services.TechStacks.Models;
using AI.CodeAgent.Builder.Application.Services.TechStacks.Queries;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Exceptions;

namespace AI.CodeAgent.Builder.Application.Services.TechStacks;

/// <summary>
/// Application service for managing TechStack entities.
/// Implements use cases for tech stack CRUD operations.
/// </summary>
public sealed class TechStackService
{
    private readonly ITechStackRepository _techStackRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProjectProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TechStackService(
        ITechStackRepository techStackRepository,
        ICategoryRepository categoryRepository,
        IProjectProfileRepository profileRepository,
        IUnitOfWork unitOfWork)
    {
        _techStackRepository = techStackRepository ?? throw new ArgumentNullException(nameof(techStackRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new tech stack.
    /// Validates category exists and tech stack name is unique within category.
    /// </summary>
    public async Task<Result<TechStackDto>> CreateTechStackAsync(
        CreateTechStackCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<TechStackDto>.Failure($"Category with ID '{command.CategoryId}' not found.");
            }

            if (!category.IsActive)
            {
                return Result<TechStackDto>.Failure($"Cannot add tech stack to inactive category '{category.Name}'.");
            }

            // Check for duplicate name within category
            if (await _techStackRepository.ExistsByNameAsync(command.CategoryId, command.Name, cancellationToken))
            {
                return Result<TechStackDto>.Failure(
                    $"A tech stack with the name '{command.Name}' already exists in category '{category.Name}'.");
            }

            // Create domain entity
            var techStack = TechStack.Create(command.CategoryId, command.Name, command.Description);

            if (!string.IsNullOrWhiteSpace(command.DefaultVersion))
                techStack.SetDefaultVersion(command.DefaultVersion);

            if (!string.IsNullOrWhiteSpace(command.DocumentationUrl))
                techStack.SetDocumentationUrl(command.DocumentationUrl);

            // Persist
            await _techStackRepository.AddAsync(techStack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(techStack, category.Name);

            return Result<TechStackDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<TechStackDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<TechStackDto>.Failure($"Failed to create tech stack: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing tech stack.
    /// </summary>
    public async Task<Result<TechStackDto>> UpdateTechStackAsync(
        UpdateTechStackCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve tech stack with parameters
            var techStack = await _techStackRepository.GetWithParametersAsync(command.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result<TechStackDto>.Failure($"Tech stack with ID '{command.TechStackId}' not found.");
            }

            // Get category name
            var category = await _categoryRepository.GetByIdAsync(techStack.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<TechStackDto>.Failure($"Category not found for tech stack '{techStack.Name}'.");
            }

            // Check for duplicate name (if name changed)
            if (!techStack.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _techStackRepository.ExistsByNameAsync(techStack.CategoryId, command.Name, cancellationToken))
                {
                    return Result<TechStackDto>.Failure(
                        $"A tech stack with the name '{command.Name}' already exists in category '{category.Name}'.");
                }
            }

            // Update entity
            techStack.SetName(command.Name);
            techStack.SetDescription(command.Description);

            if (!string.IsNullOrWhiteSpace(command.DefaultVersion))
                techStack.SetDefaultVersion(command.DefaultVersion);

            if (!string.IsNullOrWhiteSpace(command.DocumentationUrl))
                techStack.SetDocumentationUrl(command.DocumentationUrl);

            // Persist
            await _techStackRepository.UpdateAsync(techStack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(techStack, category.Name);

            return Result<TechStackDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<TechStackDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<TechStackDto>.Failure($"Failed to update tech stack: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a parameter to a tech stack.
    /// </summary>
    public async Task<Result<TechStackDto>> AddParameterAsync(
        AddStackParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve tech stack
            var techStack = await _techStackRepository.GetWithParametersAsync(command.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result<TechStackDto>.Failure($"Tech stack with ID '{command.TechStackId}' not found.");
            }

            // Create parameter
            var parameter = StackParameter.Create(
                command.ParameterName,
                command.Description,
                command.ParameterType);

            if (command.IsRequired)
                parameter.SetRequired(true);

            if (!string.IsNullOrWhiteSpace(command.DefaultValue))
                parameter.SetDefaultValue(command.DefaultValue);

            if (command.AllowedValues.Any())
                parameter.SetAllowedValues(command.AllowedValues.ToArray());

            // Add to tech stack
            techStack.AddParameter(parameter);

            // Persist
            await _techStackRepository.UpdateAsync(techStack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get category name
            var category = await _categoryRepository.GetByIdAsync(techStack.CategoryId, cancellationToken);
            var categoryName = category?.Name ?? "Unknown";

            // Map to DTO
            var dto = MapToDto(techStack, categoryName);

            return Result<TechStackDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<TechStackDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<TechStackDto>.Failure($"Failed to add parameter: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a tech stack.
    /// Tech stacks used in project profiles cannot be deleted.
    /// </summary>
    public async Task<Result> DeleteTechStackAsync(
        DeleteTechStackCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve tech stack
            var techStack = await _techStackRepository.GetByIdAsync(command.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result.Failure($"Tech stack with ID '{command.TechStackId}' not found.");
            }

            // Check if tech stack is used in any profiles
            var profiles = await _profileRepository.GetProfilesByTechStackAsync(command.TechStackId, cancellationToken);
            if (profiles.Any())
            {
                return Result.Failure(
                    $"Cannot delete tech stack '{techStack.Name}' because it is used in {profiles.Count()} project profile(s).");
            }

            // Delete
            await _techStackRepository.DeleteAsync(techStack.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete tech stack: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves tech stacks by category.
    /// </summary>
    public async Task<Result<IEnumerable<TechStackDto>>> ListTechStacksByCategoryAsync(
        ListTechStacksByCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(query.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<IEnumerable<TechStackDto>>.Failure($"Category with ID '{query.CategoryId}' not found.");
            }

            // Get tech stacks
            var techStacks = await _techStackRepository.GetByCategoryIdAsync(query.CategoryId, cancellationToken);

            // Map to DTOs
            var dtos = techStacks.Select(ts => MapToDto(ts, category.Name)).ToList();

            return Result<IEnumerable<TechStackDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TechStackDto>>.Failure($"Failed to retrieve tech stacks: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a tech stack by ID with all parameters.
    /// </summary>
    public async Task<Result<TechStackDto>> GetTechStackByIdAsync(
        GetTechStackByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var techStack = await _techStackRepository.GetWithParametersAsync(query.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result<TechStackDto>.Failure($"Tech stack with ID '{query.TechStackId}' not found.");
            }

            // Get category name
            var category = await _categoryRepository.GetByIdAsync(techStack.CategoryId, cancellationToken);
            var categoryName = category?.Name ?? "Unknown";

            var dto = MapToDto(techStack, categoryName);

            return Result<TechStackDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TechStackDto>.Failure($"Failed to retrieve tech stack: {ex.Message}");
        }
    }

    #region Mapping

    private static TechStackDto MapToDto(TechStack techStack, string categoryName)
    {
        return new TechStackDto
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
    }

    #endregion
}
