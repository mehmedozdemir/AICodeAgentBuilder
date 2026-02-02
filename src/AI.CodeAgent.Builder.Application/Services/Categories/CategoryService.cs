using AI.CodeAgent.Builder.Application.Common;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Services.Categories.Commands;
using AI.CodeAgent.Builder.Application.Services.Categories.Models;
using AI.CodeAgent.Builder.Application.Services.Categories.Queries;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Exceptions;

namespace AI.CodeAgent.Builder.Application.Services.Categories;

/// <summary>
/// Application service for managing Category entities.
/// Implements use cases for category CRUD operations.
/// </summary>
public sealed class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITechStackRepository _techStackRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ITechStackRepository techStackRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _techStackRepository = techStackRepository ?? throw new ArgumentNullException(nameof(techStackRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new category.
    /// Validates that the category name is unique.
    /// </summary>
    public async Task<Result<CategoryDto>> CreateCategoryAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for duplicate name
            if (await _categoryRepository.ExistsByNameAsync(command.Name, cancellationToken))
            {
                return Result<CategoryDto>.Failure($"A category with the name '{command.Name}' already exists.");
            }

            // Create domain entity
            var category = Category.Create(command.Name, command.Description, command.IsAIGenerated);

            // Persist
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(category);

            return Result<CategoryDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<CategoryDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Failed to create category: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing category.
    /// Validates that the new name is unique (if changed).
    /// </summary>
    public async Task<Result<CategoryDto>> UpdateCategoryAsync(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve existing category
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<CategoryDto>.Failure($"Category with ID '{command.CategoryId}' not found.");
            }

            // Check for duplicate name (if name changed)
            if (!category.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _categoryRepository.ExistsByNameAsync(command.Name, cancellationToken))
                {
                    return Result<CategoryDto>.Failure($"A category with the name '{command.Name}' already exists.");
                }
            }

            // Update entity
            category.SetName(command.Name);
            category.SetDescription(command.Description);

            // Persist
            await _categoryRepository.UpdateAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(category);

            return Result<CategoryDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<CategoryDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Failed to update category: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes (deactivates) a category.
    /// Categories with tech stacks cannot be deleted.
    /// </summary>
    public async Task<Result> DeleteCategoryAsync(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve category
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result.Failure($"Category with ID '{command.CategoryId}' not found.");
            }

            // Check if category has tech stacks
            var techStacks = await _techStackRepository.GetByCategoryIdAsync(command.CategoryId, cancellationToken);
            if (techStacks.Any())
            {
                return Result.Failure($"Cannot delete category '{category.Name}' because it contains {techStacks.Count()} tech stack(s).");
            }

            // Soft delete
            category.Deactivate();

            // Persist
            await _categoryRepository.UpdateAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete category: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves all active categories.
    /// </summary>
    public async Task<Result<IEnumerable<CategoryDto>>> ListCategoriesAsync(
        ListCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Category> categories;

            if (query.IncludeInactive)
            {
                // Get all categories
                categories = await _categoryRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                // Get only active categories
                categories = await _categoryRepository.GetAllActiveCategoriesAsync(cancellationToken);
            }

            // Map to DTOs
            var dtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var techStacks = await _techStackRepository.GetByCategoryIdAsync(category.Id, cancellationToken);
                var dto = MapToDto(category, techStacks.Count());
                dtos.Add(dto);
            }

            return Result<IEnumerable<CategoryDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CategoryDto>>.Failure($"Failed to retrieve categories: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a category by ID.
    /// </summary>
    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(
        GetCategoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(query.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<CategoryDto>.Failure($"Category with ID '{query.CategoryId}' not found.");
            }

            var techStacks = await _techStackRepository.GetByCategoryIdAsync(category.Id, cancellationToken);
            var dto = MapToDto(category, techStacks.Count());

            return Result<CategoryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Failed to retrieve category: {ex.Message}");
        }
    }

    #region Mapping

    private static CategoryDto MapToDto(Category category, int techStackCount = 0)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            IsAIGenerated = category.IsAIGenerated,
            TechStackCount = techStackCount,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt ?? category.CreatedAt
        };
    }

    #endregion
}
