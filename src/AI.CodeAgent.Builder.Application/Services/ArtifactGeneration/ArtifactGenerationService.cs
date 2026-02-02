using AI.CodeAgent.Builder.Application.Common;
using AI.CodeAgent.Builder.Application.Common.Interfaces;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Commands;
using AI.CodeAgent.Builder.Application.Services.ArtifactGeneration.Models;
using AI.CodeAgent.Builder.Domain.Entities;

namespace AI.CodeAgent.Builder.Application.Services.ArtifactGeneration;

/// <summary>
/// Application service for generating AI Code Agent artifacts.
/// Orchestrates template rendering and artifact generation from project profiles.
/// </summary>
public sealed class ArtifactGenerationService
{
    private readonly IProjectProfileRepository _profileRepository;
    private readonly ITechStackRepository _techStackRepository;
    private readonly IArchitecturePatternRepository _patternRepository;
    private readonly IEngineeringRuleRepository _ruleRepository;
    private readonly ITemplateService _templateService;

    public ArtifactGenerationService(
        IProjectProfileRepository profileRepository,
        ITechStackRepository techStackRepository,
        IArchitecturePatternRepository patternRepository,
        IEngineeringRuleRepository ruleRepository,
        ITemplateService templateService)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _techStackRepository = techStackRepository ?? throw new ArgumentNullException(nameof(techStackRepository));
        _patternRepository = patternRepository ?? throw new ArgumentNullException(nameof(patternRepository));
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    }

    /// <summary>
    /// Generates Copilot instructions markdown file.
    /// Contains tech stack details, architecture patterns, and engineering rules.
    /// </summary>
    public async Task<Result<GeneratedArtifact>> GenerateCopilotInstructionsAsync(
        GenerateCopilotInstructionsCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile with all details
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<GeneratedArtifact>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            // Validate profile is complete
            if (!profile.IsValid())
            {
                return Result<GeneratedArtifact>.Failure(
                    "Project profile is incomplete. Ensure at least one tech stack and one architecture pattern are selected.");
            }

            // Load related entities
            var techStacks = new List<TechStack>();
            foreach (var pts in profile.SelectedTechStacks)
            {
                var techStack = await _techStackRepository.GetWithParametersAsync(pts.TechStackId, cancellationToken);
                if (techStack != null)
                    techStacks.Add(techStack);
            }

            var patterns = new List<ArchitecturePattern>();
            foreach (var patternId in profile.SelectedArchitecturePatternIds)
            {
                var pattern = await _patternRepository.GetByIdAsync(patternId, cancellationToken);
                if (pattern != null)
                    patterns.Add(pattern);
            }

            var rules = new List<EngineeringRule>();
            foreach (var ruleId in profile.SelectedEngineeringRuleIds)
            {
                var rule = await _ruleRepository.GetByIdAsync(ruleId, cancellationToken);
                if (rule != null)
                    rules.Add(rule);
            }

            // Prepare template data
            var templateData = new
            {
                ProjectName = profile.ProjectName ?? profile.Name,
                Description = profile.Description,
                TeamSize = profile.TargetTeamSize,
                TechStacks = techStacks.Select(ts => new
                {
                    ts.Name,
                    ts.Description,
                    ts.DefaultVersion,
                    ts.DocumentationUrl,
                    Parameters = profile.SelectedTechStacks
                        .FirstOrDefault(pts => pts.TechStackId == ts.Id)?
                        .ParameterValues.Select(kvp => new
                        {
                            Name = kvp.Key,
                            Value = kvp.Value.Value
                        })
                }),
                ArchitecturePatterns = patterns.Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.ComplexityLevel,
                    p.ImplementationGuidelines
                }),
                EngineeringRules = rules.Select(r => new
                {
                    r.Name,
                    r.Description,
                    r.Rationale,
                    Severity = r.Constraint.Severity.ToString(),
                    Scope = r.Constraint.Scope.ToString(),
                    r.ImplementationGuidance,
                    r.ExampleCode
                }),
                GeneratedAt = DateTime.UtcNow
            };

            // Render template
            var content = await _templateService.RenderTemplateAsync("copilot-instructions", templateData, cancellationToken);

            var artifact = new GeneratedArtifact
            {
                FileName = "copilot-instructions.md",
                Content = content,
                FileType = "markdown",
                GeneratedAt = DateTime.UtcNow
            };

            return Result<GeneratedArtifact>.SuccessResult(artifact);
        }
        catch (Exception ex)
        {
            return Result<GeneratedArtifact>.Failure($"Failed to generate Copilot instructions: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates AI agent configuration file (YAML/JSON/TOML).
    /// Contains structured configuration for AI code agents.
    /// </summary>
    public async Task<Result<GeneratedArtifact>> GenerateAIAgentConfigAsync(
        GenerateAIAgentConfigCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                return Result<GeneratedArtifact>.Failure($"Project profile with ID '{command.ProfileId}' not found.");
            }

            if (!profile.IsValid())
            {
                return Result<GeneratedArtifact>.Failure("Project profile is incomplete.");
            }

            // Load tech stacks
            var techStacks = new List<TechStack>();
            foreach (var pts in profile.SelectedTechStacks)
            {
                var techStack = await _techStackRepository.GetWithParametersAsync(pts.TechStackId, cancellationToken);
                if (techStack != null)
                    techStacks.Add(techStack);
            }

            // Prepare config data
            var configData = new
            {
                Project = new
                {
                    Name = profile.ProjectName ?? profile.Name,
                    Description = profile.Description,
                    TeamSize = profile.TargetTeamSize
                },
                TechStacks = techStacks.Select(ts => new
                {
                    Name = ts.Name,
                    Version = ts.DefaultVersion,
                    Configuration = profile.SelectedTechStacks
                        .FirstOrDefault(pts => pts.TechStackId == ts.Id)?
                        .ParameterValues.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Value)
                }),
                Patterns = profile.SelectedArchitecturePatternIds.ToList(),
                Rules = profile.SelectedEngineeringRuleIds.ToList(),
                GeneratedAt = DateTime.UtcNow
            };

            // Render template based on format
            var templateName = command.ConfigFormat switch
            {
                "yaml" => "aiagent-config-yaml",
                "json" => "aiagent-config-json",
                "toml" => "aiagent-config-toml",
                _ => "aiagent-config-yaml"
            };

            var content = await _templateService.RenderTemplateAsync(templateName, configData, cancellationToken);

            var fileExtension = command.ConfigFormat switch
            {
                "json" => "json",
                "toml" => "toml",
                _ => "yaml"
            };

            var artifact = new GeneratedArtifact
            {
                FileName = $"aiagent.config.{fileExtension}",
                Content = content,
                FileType = command.ConfigFormat,
                GeneratedAt = DateTime.UtcNow
            };

            return Result<GeneratedArtifact>.SuccessResult(artifact);
        }
        catch (Exception ex)
        {
            return Result<GeneratedArtifact>.Failure($"Failed to generate AI agent config: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates all engineering artifacts (policies, templates, guidelines).
    /// Returns multiple files as a collection.
    /// </summary>
    public async Task<Result<ArtifactGenerationResult>> GenerateEngineeringArtifactsAsync(
        GenerateEngineeringArtifactsCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = new ArtifactGenerationResult
            {
                IsSuccess = true,
                GeneratedAt = DateTime.UtcNow
            };

            // Retrieve profile
            var profile = await _profileRepository.GetWithDetailsAsync(command.ProfileId, cancellationToken);
            if (profile == null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Project profile with ID '{command.ProfileId}' not found.";
                return Result<ArtifactGenerationResult>.Failure(result.ErrorMessage);
            }

            if (!profile.IsValid())
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Project profile is incomplete.";
                return Result<ArtifactGenerationResult>.Failure(result.ErrorMessage);
            }

            // Load engineering rules
            var rules = new List<EngineeringRule>();
            foreach (var ruleId in profile.SelectedEngineeringRuleIds)
            {
                var rule = await _ruleRepository.GetByIdAsync(ruleId, cancellationToken);
                if (rule != null)
                    rules.Add(rule);
            }

            // Generate engineering policy document
            var policyData = new
            {
                ProjectName = profile.ProjectName ?? profile.Name,
                Rules = rules.Select(r => new
                {
                    r.Name,
                    r.Description,
                    r.Rationale,
                    Severity = r.Constraint.Severity.ToString(),
                    Scope = r.Constraint.Scope.ToString(),
                    r.ImplementationGuidance,
                    IsEnforced = r.IsEnforced
                }),
                GeneratedAt = DateTime.UtcNow
            };

            var policyContent = await _templateService.RenderTemplateAsync("engineering-policy", policyData, cancellationToken);
            result.Artifacts.Add(new GeneratedArtifact
            {
                FileName = "ENGINEERING_POLICY.md",
                Content = policyContent,
                FileType = "markdown",
                GeneratedAt = DateTime.UtcNow
            });

            // Generate code review checklist
            var checklistData = new
            {
                ProjectName = profile.ProjectName ?? profile.Name,
                Rules = rules.Where(r => r.IsEnforced).Select(r => new
                {
                    r.Name,
                    r.Description,
                    Scope = r.Constraint.Scope.ToString()
                }),
                GeneratedAt = DateTime.UtcNow
            };

            var checklistContent = await _templateService.RenderTemplateAsync("code-review-checklist", checklistData, cancellationToken);
            result.Artifacts.Add(new GeneratedArtifact
            {
                FileName = "CODE_REVIEW_CHECKLIST.md",
                Content = checklistContent,
                FileType = "markdown",
                GeneratedAt = DateTime.UtcNow
            });

            // Generate README template
            var techStacks = new List<TechStack>();
            foreach (var pts in profile.SelectedTechStacks)
            {
                var techStack = await _techStackRepository.GetWithParametersAsync(pts.TechStackId, cancellationToken);
                if (techStack != null)
                    techStacks.Add(techStack);
            }

            var patterns = new List<ArchitecturePattern>();
            foreach (var patternId in profile.SelectedArchitecturePatternIds)
            {
                var pattern = await _patternRepository.GetByIdAsync(patternId, cancellationToken);
                if (pattern != null)
                    patterns.Add(pattern);
            }

            var readmeData = new
            {
                ProjectName = profile.ProjectName ?? profile.Name,
                Description = profile.Description,
                TechStacks = techStacks.Select(ts => new
                {
                    ts.Name,
                    ts.DefaultVersion,
                    ts.DocumentationUrl
                }),
                ArchitecturePatterns = patterns.Select(p => new
                {
                    p.Name,
                    p.Description
                }),
                GeneratedAt = DateTime.UtcNow
            };

            var readmeContent = await _templateService.RenderTemplateAsync("readme-template", readmeData, cancellationToken);
            result.Artifacts.Add(new GeneratedArtifact
            {
                FileName = "README.md",
                Content = readmeContent,
                FileType = "markdown",
                GeneratedAt = DateTime.UtcNow
            });

            return Result<ArtifactGenerationResult>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            var result = new ArtifactGenerationResult
            {
                IsSuccess = false,
                ErrorMessage = $"Failed to generate engineering artifacts: {ex.Message}",
                GeneratedAt = DateTime.UtcNow
            };
            return Result<ArtifactGenerationResult>.Failure(result.ErrorMessage);
        }
    }
}
