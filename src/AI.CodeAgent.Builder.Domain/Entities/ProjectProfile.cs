using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.ValueObjects;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents a user-defined project configuration.
/// This is the primary aggregate root that composes selections of:
/// - Categories
/// - TechStacks with their parameter values
/// - ArchitecturePatterns
/// - EngineeringRules
/// 
/// ProjectProfile enforces invariants and ensures consistency across all selections.
/// It serves as the basis for generating AI Code Agent infrastructure.
/// </summary>
public sealed class ProjectProfile : BaseEntity
{
    private readonly List<ProfileTechStack> _techStacks = new();
    private readonly List<Guid> _architecturePatternIds = new();
    private readonly List<Guid> _engineeringRuleIds = new();
    private string _name = string.Empty;
    private string _description = string.Empty;

    // Private constructor for EF Core
    private ProjectProfile()
    {
    }

    private ProjectProfile(string name, string description)
    {
        SetName(name);
        SetDescription(description);
        IsActive = true;
    }

    /// <summary>
    /// The name of the project profile.
    /// Must be unique per user or organization.
    /// </summary>
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    /// <summary>
    /// A detailed description of this project profile and its purpose.
    /// </summary>
    public string Description
    {
        get => _description;
        private set => _description = value;
    }

    /// <summary>
    /// Project name that will be used in generated artifacts.
    /// </summary>
    public string? ProjectName { get; private set; }

    /// <summary>
    /// Target team size for this project.
    /// Used to validate architecture pattern suitability.
    /// </summary>
    public int? TargetTeamSize { get; private set; }

    /// <summary>
    /// Indicates whether this profile is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Technology stacks selected for this project with their configured parameters.
    /// </summary>
    public IReadOnlyCollection<ProfileTechStack> TechStacks => _techStacks.AsReadOnly();

    /// <summary>
    /// Architecture pattern IDs selected for this project.
    /// </summary>
    public IReadOnlyCollection<Guid> ArchitecturePatternIds => _architecturePatternIds.AsReadOnly();

    /// <summary>
    /// Engineering rule IDs selected for this project.
    /// </summary>
    public IReadOnlyCollection<Guid> EngineeringRuleIds => _engineeringRuleIds.AsReadOnly();

    /// <summary>
    /// Factory method to create a new project profile.
    /// </summary>
    public static ProjectProfile Create(string name, string description)
    {
        return new ProjectProfile(name, description);
    }

    /// <summary>
    /// Updates the profile name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project profile name cannot be empty.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Project profile name cannot exceed 200 characters.", nameof(name));

        _name = name.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the profile description.
    /// </summary>
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project profile description cannot be empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Project profile description cannot exceed 1000 characters.", nameof(description));

        _description = description.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the project name that will be used in generated code.
    /// </summary>
    public void SetProjectName(string? projectName)
    {
        if (!string.IsNullOrWhiteSpace(projectName) && projectName.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters.", nameof(projectName));

        ProjectName = projectName?.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the target team size.
    /// </summary>
    public void SetTargetTeamSize(int? teamSize)
    {
        if (teamSize.HasValue && teamSize.Value <= 0)
            throw new ArgumentException("Team size must be positive.", nameof(teamSize));

        TargetTeamSize = teamSize;
        SetUpdatedAt();
    }

    /// <summary>
    /// Adds a tech stack to this profile with parameter values.
    /// </summary>
    public void AddTechStack(Guid techStackId, Dictionary<string, ParameterValue>? parameterValues = null)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID cannot be empty.", nameof(techStackId));

        if (_techStacks.Any(ts => ts.TechStackId == techStackId))
            throw new InvalidOperationException($"Tech stack with ID '{techStackId}' is already added to this profile.");

        var profileTechStack = ProfileTechStack.Create(techStackId, parameterValues);
        _techStacks.Add(profileTechStack);
        SetUpdatedAt();
    }

    /// <summary>
    /// Removes a tech stack from this profile.
    /// </summary>
    public void RemoveTechStack(Guid techStackId)
    {
        var techStack = _techStacks.FirstOrDefault(ts => ts.TechStackId == techStackId);
        if (techStack == null)
            throw new InvalidOperationException($"Tech stack with ID '{techStackId}' is not in this profile.");

        _techStacks.Remove(techStack);
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates parameter values for a tech stack.
    /// </summary>
    public void UpdateTechStackParameters(Guid techStackId, Dictionary<string, ParameterValue> parameterValues)
    {
        var techStack = _techStacks.FirstOrDefault(ts => ts.TechStackId == techStackId);
        if (techStack == null)
            throw new InvalidOperationException($"Tech stack with ID '{techStackId}' is not in this profile.");

        techStack.SetParameterValues(parameterValues);
        SetUpdatedAt();
    }

    /// <summary>
    /// Adds an architecture pattern to this profile.
    /// </summary>
    public void AddArchitecturePattern(Guid patternId)
    {
        if (patternId == Guid.Empty)
            throw new ArgumentException("Architecture pattern ID cannot be empty.", nameof(patternId));

        if (_architecturePatternIds.Contains(patternId))
            throw new InvalidOperationException($"Architecture pattern with ID '{patternId}' is already added.");

        _architecturePatternIds.Add(patternId);
        SetUpdatedAt();
    }

    /// <summary>
    /// Removes an architecture pattern from this profile.
    /// </summary>
    public void RemoveArchitecturePattern(Guid patternId)
    {
        if (!_architecturePatternIds.Remove(patternId))
            throw new InvalidOperationException($"Architecture pattern with ID '{patternId}' is not in this profile.");

        SetUpdatedAt();
    }

    /// <summary>
    /// Adds an engineering rule to this profile.
    /// </summary>
    public void AddEngineeringRule(Guid ruleId)
    {
        if (ruleId == Guid.Empty)
            throw new ArgumentException("Engineering rule ID cannot be empty.", nameof(ruleId));

        if (_engineeringRuleIds.Contains(ruleId))
            throw new InvalidOperationException($"Engineering rule with ID '{ruleId}' is already added.");

        _engineeringRuleIds.Add(ruleId);
        SetUpdatedAt();
    }

    /// <summary>
    /// Removes an engineering rule from this profile.
    /// </summary>
    public void RemoveEngineeringRule(Guid ruleId)
    {
        if (!_engineeringRuleIds.Remove(ruleId))
            throw new InvalidOperationException($"Engineering rule with ID '{ruleId}' is not in this profile.");

        SetUpdatedAt();
    }

    /// <summary>
    /// Clears all selections in this profile.
    /// </summary>
    public void ClearAllSelections()
    {
        _techStacks.Clear();
        _architecturePatternIds.Clear();
        _engineeringRuleIds.Clear();
        SetUpdatedAt();
    }

    /// <summary>
    /// Validates that the profile is complete and ready for code generation.
    /// </summary>
    public bool IsValid()
    {
        return _techStacks.Any() 
            && _architecturePatternIds.Any() 
            && !string.IsNullOrWhiteSpace(ProjectName);
    }

    /// <summary>
    /// Activates the profile.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivates the profile.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}

/// <summary>
/// Represents a tech stack within a project profile with its configured parameters.
/// This is an owned entity that cannot exist independently of ProjectProfile.
/// </summary>
public sealed class ProfileTechStack : BaseEntity
{
    private Dictionary<string, ParameterValue> _parameterValues = new();

    // Private constructor for EF Core
    private ProfileTechStack()
    {
    }

    private ProfileTechStack(Guid techStackId)
    {
        if (techStackId == Guid.Empty)
            throw new ArgumentException("Tech stack ID cannot be empty.", nameof(techStackId));

        TechStackId = techStackId;
    }

    /// <summary>
    /// The ID of the tech stack.
    /// </summary>
    public Guid TechStackId { get; private set; }

    /// <summary>
    /// Parameter values configured for this tech stack.
    /// Key: parameter name, Value: parameter value.
    /// </summary>
    public IReadOnlyDictionary<string, ParameterValue> ParameterValues => _parameterValues;

    /// <summary>
    /// Factory method to create a profile tech stack.
    /// </summary>
    public static ProfileTechStack Create(Guid techStackId, Dictionary<string, ParameterValue>? parameterValues = null)
    {
        var profileTechStack = new ProfileTechStack(techStackId);
        
        if (parameterValues != null && parameterValues.Any())
        {
            profileTechStack.SetParameterValues(parameterValues);
        }

        return profileTechStack;
    }

    /// <summary>
    /// Sets or updates parameter values.
    /// </summary>
    public void SetParameterValues(Dictionary<string, ParameterValue> parameterValues)
    {
        if (parameterValues == null)
            throw new ArgumentNullException(nameof(parameterValues));

        _parameterValues = new Dictionary<string, ParameterValue>(parameterValues);
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets a single parameter value.
    /// </summary>
    public void SetParameterValue(string parameterName, ParameterValue value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("Parameter name cannot be empty.", nameof(parameterName));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        _parameterValues[parameterName] = value;
        SetUpdatedAt();
    }

    /// <summary>
    /// Gets a parameter value by name.
    /// </summary>
    public ParameterValue? GetParameterValue(string parameterName)
    {
        return _parameterValues.TryGetValue(parameterName, out var value) ? value : null;
    }

    /// <summary>
    /// Removes a parameter value.
    /// </summary>
    public void RemoveParameterValue(string parameterName)
    {
        if (_parameterValues.Remove(parameterName))
        {
            SetUpdatedAt();
        }
    }
}
