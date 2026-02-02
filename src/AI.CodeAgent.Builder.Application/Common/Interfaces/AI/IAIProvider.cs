namespace AI.CodeAgent.Builder.Application.Common.Interfaces.AI;

/// <summary>
/// Interface for AI provider abstraction.
/// Decouples application logic from specific AI implementations (OpenAI, Azure OpenAI, etc.).
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Sends a prompt to the AI provider and returns the structured response.
    /// </summary>
    /// <param name="request">The AI prompt request with context and parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI response with generated content and metadata.</returns>
    Task<AIPromptResponse> SendPromptAsync(AIPromptRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the AI provider is properly configured and reachable.
    /// </summary>
    Task<bool> ValidateConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the name of the AI provider (e.g., "OpenAI GPT-4", "Azure OpenAI").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Gets the model being used (e.g., "gpt-4", "gpt-3.5-turbo").
    /// </summary>
    string ModelName { get; }
}
