namespace AI.CodeAgent.Builder.Application.Common.Interfaces.Templates;

/// <summary>
/// Service for rendering templates using Scriban template engine.
/// Supports file-based templates with caching.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Renders a template with the provided model.
    /// </summary>
    /// <param name="templateName">Name of the template file (without .scriban extension)</param>
    /// <param name="model">Model object to pass to the template</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered template as string</returns>
    Task<string> RenderTemplateAsync(string templateName, object model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a template exists.
    /// </summary>
    Task<bool> TemplateExistsAsync(string templateName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all available templates.
    /// </summary>
    Task<IReadOnlyList<string>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the template cache (if caching is enabled).
    /// </summary>
    Task ClearCacheAsync(CancellationToken cancellationToken = default);
}
