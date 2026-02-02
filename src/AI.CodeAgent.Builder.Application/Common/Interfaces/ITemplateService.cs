namespace AI.CodeAgent.Builder.Application.Common.Interfaces;

/// <summary>
/// Interface for template rendering service.
/// Used to generate configuration files and artifacts using Scriban templates.
/// </summary>
public interface ITemplateService
{
    Task<string> RenderTemplateAsync(string templateContent, object model, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateFromFileAsync(string templatePath, object model, CancellationToken cancellationToken = default);
}
