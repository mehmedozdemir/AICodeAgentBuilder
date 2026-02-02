namespace AI.CodeAgent.Builder.Application.Common.Interfaces.AI;

/// <summary>
/// Factory for creating AI provider instances.
/// Supports runtime provider selection based on configuration.
/// </summary>
public interface IAIProviderFactory
{
    /// <summary>
    /// Creates the default AI provider based on configuration.
    /// </summary>
    IAIProvider CreateProvider();

    /// <summary>
    /// Creates a specific AI provider by name.
    /// </summary>
    /// <param name="providerName">Name of the provider (OpenAI, AzureOpenAI, Local)</param>
    IAIProvider CreateProvider(string providerName);
}
