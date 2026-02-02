using AI.CodeAgent.Builder.Application.Common.Interfaces.Templates;
using AI.CodeAgent.Builder.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scriban;
using Scriban.Runtime;

namespace AI.CodeAgent.Builder.Infrastructure.Templates;

/// <summary>
/// Scriban-based template service for generating artifacts.
/// Loads templates from filesystem with caching support.
/// </summary>
public sealed class TemplateService : ITemplateService
{
    private readonly TemplateSettings _settings;
    private readonly ILogger<TemplateService> _logger;
    private readonly Dictionary<string, Template> _templateCache = new();
    private readonly object _cacheLock = new();

    public TemplateService(
        IOptions<TemplateSettings> settings,
        ILogger<TemplateService> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        EnsureTemplateDirectoryExists();
    }

    public async Task<string> RenderTemplateAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await GetOrLoadTemplateAsync(templateName, cancellationToken);

            var scriptObject = new ScriptObject();
            scriptObject.Import(model, renamer: member => member.Name);

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            var result = await template.RenderAsync(context);

            _logger.LogInformation("Template '{TemplateName}' rendered successfully", templateName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render template '{TemplateName}'", templateName);

            if (_settings.FailFastOnError)
            {
                throw new InvalidOperationException($"Template rendering failed for '{templateName}'", ex);
            }

            return $"<!-- Template rendering failed: {ex.Message} -->";
        }
    }

    public Task<bool> TemplateExistsAsync(string templateName, CancellationToken cancellationToken = default)
    {
        var templatePath = GetTemplatePath(templateName);
        var exists = File.Exists(templatePath);
        return Task.FromResult(exists);
    }

    public async Task<IReadOnlyList<string>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templateDirectory = _settings.TemplateDirectory;

        if (!Directory.Exists(templateDirectory))
        {
            _logger.LogWarning("Template directory does not exist: {TemplateDirectory}", templateDirectory);
            return Array.Empty<string>();
        }

        var templateFiles = Directory.GetFiles(templateDirectory, "*.scriban", SearchOption.AllDirectories);

        var templateNames = templateFiles
            .Select(f => Path.GetRelativePath(templateDirectory, f))
            .Select(f => Path.ChangeExtension(f, null)) // Remove .scriban extension
            .ToList();

        return templateNames;
    }

    public Task ClearCacheAsync(CancellationToken cancellationToken = default)
    {
        lock (_cacheLock)
        {
            _templateCache.Clear();
            _logger.LogInformation("Template cache cleared");
        }

        return Task.CompletedTask;
    }

    private async Task<Template> GetOrLoadTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        if (_settings.EnableCaching)
        {
            lock (_cacheLock)
            {
                if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
                {
                    _logger.LogDebug("Template '{TemplateName}' loaded from cache", templateName);
                    return cachedTemplate;
                }
            }
        }

        var templatePath = GetTemplatePath(templateName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template not found: {templatePath}", templatePath);
        }

        var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
        var template = Template.Parse(templateContent);

        if (template.HasErrors)
        {
            var errors = string.Join("; ", template.Messages.Select(m => m.Message));
            throw new InvalidOperationException($"Template '{templateName}' has syntax errors: {errors}");
        }

        if (_settings.EnableCaching)
        {
            lock (_cacheLock)
            {
                _templateCache[templateName] = template;
                _logger.LogDebug("Template '{TemplateName}' cached", templateName);
            }
        }

        return template;
    }

    private string GetTemplatePath(string templateName)
    {
        // Normalize template name
        var fileName = templateName.EndsWith(".scriban", StringComparison.OrdinalIgnoreCase)
            ? templateName
            : $"{templateName}.scriban";

        return Path.Combine(_settings.TemplateDirectory, fileName);
    }

    private void EnsureTemplateDirectoryExists()
    {
        if (!Directory.Exists(_settings.TemplateDirectory))
        {
            _logger.LogWarning(
                "Template directory does not exist and will be created: {TemplateDirectory}",
                _settings.TemplateDirectory);

            Directory.CreateDirectory(_settings.TemplateDirectory);
        }
    }
}
