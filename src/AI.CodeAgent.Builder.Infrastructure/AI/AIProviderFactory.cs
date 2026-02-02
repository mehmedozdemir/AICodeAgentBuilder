using AI.CodeAgent.Builder.Application.Common.Interfaces.AI;
using AI.CodeAgent.Builder.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.CodeAgent.Builder.Infrastructure.AI;

/// <summary>
/// Factory implementation for creating AI provider instances based on configuration.
/// </summary>
public sealed class AIProviderFactory : IAIProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AIProviderSettings _settings;

    public AIProviderFactory(IServiceProvider serviceProvider, IOptions<AIProviderSettings> settings)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public IAIProvider CreateProvider()
    {
        return CreateProvider(_settings.Provider);
    }

    public IAIProvider CreateProvider(string providerName)
    {
        return providerName.ToLowerInvariant() switch
        {
            "openai" => _serviceProvider.GetRequiredService<OpenAIProvider>(),
            "azureopenai" => _serviceProvider.GetRequiredService<AzureOpenAIProvider>(),
            _ => throw new ArgumentException($"Unknown AI provider: {providerName}", nameof(providerName))
        };
    }
}
