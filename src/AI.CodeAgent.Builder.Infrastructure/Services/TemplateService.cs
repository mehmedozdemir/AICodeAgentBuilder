using AI.CodeAgent.Builder.Application.Common.Interfaces;
using Scriban;

namespace AI.CodeAgent.Builder.Infrastructure.Services;

/// <summary>
/// Service for rendering Scriban templates.
/// Used to generate copilot-instructions, configuration files, and other artifacts.
/// </summary>
public sealed class TemplateService : ITemplateService
{
    public Task<string> RenderTemplateAsync(string templateContent, object model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(templateContent))
            throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));

        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var template = Template.Parse(templateContent);
        
        if (template.HasErrors)
        {
            var errors = string.Join(Environment.NewLine, template.Messages);
            throw new InvalidOperationException($"Template parsing failed: {errors}");
        }

        var result = template.Render(model);
        return Task.FromResult(result);
    }

    public async Task<string> RenderTemplateFromFileAsync(string templatePath, object model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template file not found: {templatePath}");

        var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
        return await RenderTemplateAsync(templateContent, model, cancellationToken);
    }
}
