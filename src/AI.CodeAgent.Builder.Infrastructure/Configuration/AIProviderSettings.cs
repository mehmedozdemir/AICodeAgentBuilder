namespace AI.CodeAgent.Builder.Infrastructure.Configuration;

/// <summary>
/// AI provider configuration settings.
/// Supports multiple providers: OpenAI, Azure OpenAI, and future local LLMs.
/// </summary>
public sealed class AIProviderSettings
{
    public const string SectionName = "AIProvider";

    /// <summary>
    /// Active provider: OpenAI, AzureOpenAI, or LocalLLM.
    /// </summary>
    public string Provider { get; set; } = "OpenAI";

    /// <summary>
    /// OpenAI-specific settings.
    /// </summary>
    public OpenAISettings OpenAI { get; set; } = new();

    /// <summary>
    /// Azure OpenAI-specific settings.
    /// </summary>
    public AzureOpenAISettings AzureOpenAI { get; set; } = new();

    /// <summary>
    /// Default model to use (e.g., gpt-4, gpt-3.5-turbo).
    /// </summary>
    public string DefaultModel { get; set; } = "gpt-4";

    /// <summary>
    /// Default temperature (0.0 - 1.0).
    /// Lower = more deterministic, Higher = more creative.
    /// </summary>
    public double DefaultTemperature { get; set; } = 0.7;

    /// <summary>
    /// Default maximum tokens for completion.
    /// </summary>
    public int DefaultMaxTokens { get; set; } = 2000;

    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum retry attempts on failure.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Initial retry delay in milliseconds.
    /// Uses exponential backoff: delay * 2^attempt.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;
}

/// <summary>
/// OpenAI-specific configuration.
/// </summary>
public sealed class OpenAISettings
{
    /// <summary>
    /// OpenAI API key.
    /// Should be stored in user secrets or environment variable.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Organization ID (optional).
    /// </summary>
    public string? OrganizationId { get; set; }

    /// <summary>
    /// API base URL (optional, defaults to OpenAI).
    /// </summary>
    public string? BaseUrl { get; set; }
}

/// <summary>
/// Azure OpenAI-specific configuration.
/// </summary>
public sealed class AzureOpenAISettings
{
    /// <summary>
    /// Azure OpenAI API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI endpoint (e.g., https://YOUR_RESOURCE.openai.azure.com/).
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Deployment name (e.g., gpt-4-deployment).
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// API version (e.g., 2023-05-15).
    /// </summary>
    public string ApiVersion { get; set; } = "2024-02-01";
}
