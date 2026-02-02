namespace AI.CodeAgent.Builder.Infrastructure.Configuration;

/// <summary>
/// Template engine configuration settings.
/// </summary>
public sealed class TemplateSettings
{
    public const string SectionName = "Templates";

    /// <summary>
    /// Base directory for template files.
    /// Relative to application root or absolute path.
    /// </summary>
    public string TemplateDirectory { get; set; } = "Templates";

    /// <summary>
    /// Enable template caching for performance.
    /// Recommended for production.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Fail fast on template errors.
    /// If true, throws exception immediately on template error.
    /// If false, returns error message in output.
    /// </summary>
    public bool FailFastOnError { get; set; } = true;
}
