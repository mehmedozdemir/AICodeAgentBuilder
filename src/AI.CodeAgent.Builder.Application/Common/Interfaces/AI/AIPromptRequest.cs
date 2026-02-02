namespace AI.CodeAgent.Builder.Application.Common.Interfaces.AI;

/// <summary>
/// Represents a request to an AI provider.
/// Contains the prompt, context, and execution parameters.
/// </summary>
public sealed class AIPromptRequest
{
    /// <summary>
    /// The main prompt text sent to the AI.
    /// </summary>
    public string Prompt { get; }

    /// <summary>
    /// Context about the request (e.g., "GenerateCategories", "GenerateTechStacks").
    /// Used for tracking and auditing.
    /// </summary>
    public string RequestContext { get; }

    /// <summary>
    /// Optional system message that sets the behavior/role of the AI.
    /// </summary>
    public string? SystemMessage { get; }

    /// <summary>
    /// Maximum number of tokens to generate in the response.
    /// </summary>
    public int? MaxTokens { get; }

    /// <summary>
    /// Temperature parameter controlling randomness (0.0 = deterministic, 1.0 = creative).
    /// </summary>
    public double? Temperature { get; }

    /// <summary>
    /// Expected format of the response (e.g., "JSON", "YAML", "Markdown").
    /// </summary>
    public string? ExpectedFormat { get; }

    private AIPromptRequest(
        string prompt,
        string requestContext,
        string? systemMessage,
        int? maxTokens,
        double? temperature,
        string? expectedFormat)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));

        if (string.IsNullOrWhiteSpace(requestContext))
            throw new ArgumentException("Request context cannot be empty.", nameof(requestContext));

        Prompt = prompt.Trim();
        RequestContext = requestContext.Trim();
        SystemMessage = systemMessage?.Trim();
        MaxTokens = maxTokens;
        Temperature = temperature;
        ExpectedFormat = expectedFormat?.Trim();
    }

    /// <summary>
    /// Creates a new AI prompt request.
    /// </summary>
    public static AIPromptRequest Create(
        string prompt,
        string requestContext,
        string? systemMessage = null,
        int? maxTokens = null,
        double? temperature = null,
        string? expectedFormat = null)
    {
        return new AIPromptRequest(prompt, requestContext, systemMessage, maxTokens, temperature, expectedFormat);
    }
}
