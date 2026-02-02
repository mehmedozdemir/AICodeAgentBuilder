using AI.CodeAgent.Builder.Application.Common;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Commands;
using AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Models;
using AI.CodeAgent.Builder.Application.Services.ProjectProfiles.Queries;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.Exceptions;
using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Application.Services.ProjectProfiles;

/// <summary>
/// Application service for managing ProjectProfile entities.
/// Implements use cases for project profile lifecycle management.
/// </summary>
public sealed class ProjectProfileService
{
    private readonly IProjectProfileRepository _profileRepository;
    private readonly ITechStackRepository _techStackRepository;
    private readonly IArchitecturePatternRepository _patternRepository;
    private readonly IEngineeringRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectProfileService(
        IProjectProfileRepository profileRepository,
        ITechStackRepository techStackRepository,
        IArchitecturePatternRepository patternRepository,
        IEngineeringRuleRepository ruleRepository,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _techStackRepository = techStackRepository ?? throw new ArgumentNullException(nameof(techStackRepository));
        _patternRepository = patternRepository ?? throw new ArgumentNullException(nameof(patternRepository));
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new project profile.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> CreateProjectProfileAsync(
        CreateProjectProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create domain entity
            var profile = ProjectProfile.Create(command.Name, command.Description);

            // Persist
            await _profileRepository.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<ProjectProfileDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to create project profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates project profile metadata.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> UpdateProjectProfileAsync(
        UpdateProjectProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<ProjectProfileDto>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            // Update entity
            profile.SetName(command.Name);
            profile.SetDescription(command.Description);

            if (!string.IsNullOrWhiteSpace(command.ProjectName))
                profile.SetProjectName(command.ProjectName);

            if (command.TargetTeamSize.HasValue)
                profile.SetTargetTeamSize(command.TargetTeamSize.Value);

            // Persist
            await _profileRepository.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<ProjectProfileDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to update project profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a tech stack to the profile with parameter values.
    /// Validates that all required parameters are provided with valid values.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> AddTechStackAsync(
        AddTechStackToProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<ProjectProfileDto>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            // Validate tech stack exists
            var techStack = await _techStackRepository.GetWithParametersAsync(command.TechStackId, cancellationToken);
            if (techStack == null)
            {
                return Result<ProjectProfileDto>.Failure($"Tech stack with ID '{command.TechStackId}' not found.");
            }

            // Validate required parameters are provided
            var requiredParams = techStack.Parameters.Where(p => p.IsRequired).ToList();
            foreach (var requiredParam in requiredParams)
            {
                if (!command.ParameterValues.ContainsKey(requiredParam.Name))
                {
                    return Result<ProjectProfileDto>.Failure(
                        $"Required parameter '{requiredParam.Name}' is missing for tech stack '{techStack.Name}'.");
                }
            }

            // Convert parameter values to domain value objects
            var paramValues = new Dictionary<string, ParameterValue>();
            foreach (var kvp in command.ParameterValues)
            {
                var parameter = techStack.Parameters.FirstOrDefault(
                    p => p.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));

                if (parameter == null)
                {
                    return Result<ProjectProfileDto>.Failure(
                        $"Parameter '{kvp.Key}' does not exist for tech stack '{techStack.Name}'.");
                }

                // Validate value
                try
                {
                    parameter.ValidateValue(kvp.Value);
                }
                catch (ArgumentException ex)
                {
                    return Result<ProjectProfileDto>.Failure(
                        $"Invalid value for parameter '{parameter.Name}': {ex.Message}");
                }

                // Create appropriate ParameterValue based on type
                var paramValue = parameter.ParameterType switch
                {
                    Domain.Enums.ParameterType.String => ParameterValue.CreateString(kvp.Value),
                    Domain.Enums.ParameterType.Number => ParameterValue.CreateNumber(decimal.Parse(kvp.Value)),
                    Domain.Enums.ParameterType.Boolean => ParameterValue.CreateBoolean(bool.Parse(kvp.Value)),
                    Domain.Enums.ParameterType.Enum => ParameterValue.CreateEnum(kvp.Value),
                    Domain.Enums.ParameterType.Version => ParameterValue.CreateVersion(kvp.Value),
                    _ => throw new InvalidOperationException($"Unsupported parameter type: {parameter.ParameterType}")
                };

                paramValues[parameter.Name] = paramValue;
            }

            // Add tech stack to profile
            profile.AddTechStack(command.TechStackId, paramValues);

            // Persist
            await _profileRepository.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<ProjectProfileDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to add tech stack: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds an architecture pattern to the profile.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> AddArchitecturePatternAsync(
        AddArchitecturePatternCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<ProjectProfileDto>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            // Validate pattern exists
            var pattern = await _patternRepository.GetByIdAsync(command.PatternId, cancellationToken);
            if (pattern == null)
            {
                return Result<ProjectProfileDto>.Failure($"Architecture pattern with ID '{command.PatternId}' not found.");
            }

            // Add pattern
            profile.AddArchitecturePattern(command.PatternId);

            // Persist
            await _profileRepository.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<ProjectProfileDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to add architecture pattern: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds an engineering rule to the profile.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> AddEngineeringRuleAsync(
        AddEngineeringRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<ProjectProfileDto>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            // Validate rule exists
            var rule = await _ruleRepository.GetByIdAsync(command.RuleId, cancellationToken);
            if (rule == null)
            {
                return Result<ProjectProfileDto>.Failure($"Engineering rule with ID '{command.RuleId}' not found.");
            }

            // Add rule
            profile.AddEngineeringRule(command.RuleId);

            // Persist
            await _profileRepository.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<ProjectProfileDto>.Failure($"Domain validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to add engineering rule: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a project profile with all details.
    /// </summary>
    public async Task<Result<ProjectProfileDto>> GetProjectProfileDetailsAsync(
        GetProjectProfileDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _profileRepository.GetWithDetailsAsync(query.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<ProjectProfileDto>.Failure($"Project profile with ID '{query.ProfileId}' not found.");
            }

            var dto = MapToDto(profile);

            return Result<ProjectProfileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<ProjectProfileDto>.Failure($"Failed to retrieve project profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves all project profiles.
    /// </summary>
    public async Task<Result<IEnumerable<ProjectProfileDto>>> ListProjectProfilesAsync(
        ListProjectProfilesQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profiles = await _profileRepository.GetAllProfilesAsync(cancellationToken);
            var dtos = profiles.Select(MapToDto).ToList();

            return Result<IEnumerable<ProjectProfileDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProjectProfileDto>>.Failure($"Failed to retrieve project profiles: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates that a project profile is complete and ready for artifact generation.
    /// </summary>
    public async Task<Result<bool>> ValidateProjectProfileAsync(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _profileRepository.GetWithDetailsAsync(profileId, cancellationToken);
            if (profile == null)
            {
                return Result<bool>.Failure($"Project profile with ID '{profileId}' not found.");
            }

            var isValid = profile.IsValid();

            if (!isValid)
            {
                return Result<bool>.Failure("Profile validation failed: " +
                    "Ensure at least one tech stack and one architecture pattern are selected, " +
                    "and all required parameters are provided.");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to validate project profile: {ex.Message}");
        }
    }

    #region Mapping

    private static ProjectProfileDto MapToDto(ProjectProfile profile)
    {
        return new ProjectProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Description = profile.Description,
            ProjectName = profile.ProjectName,
            TargetTeamSize = profile.TargetTeamSize,
            TechStacks = profile.TechStacks.Select(pts => new ProfileTechStackDto
            {
                TechStackId = pts.TechStackId,
                TechStackName = pts.TechStackId.ToString(), // Will be enriched by UI layer
                ParameterValues = pts.ParameterValues.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Value)
            }).ToList(),
            ArchitecturePatternIds = profile.ArchitecturePatternIds.ToList(),
            EngineeringRuleIds = profile.EngineeringRuleIds.ToList(),
            IsValid = profile.IsValid(),
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt ?? profile.CreatedAt
        };
    }

    #endregion
}
