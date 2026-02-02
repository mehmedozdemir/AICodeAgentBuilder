namespace AI.CodeAgent.Builder.Application.Common.Interfaces.AI;

/// <summary>
/// Represents a response from an AI provider.
/// Contains the generated content and metadata about the interaction.
/// </summary>
public sealed class AIPromptResponse
{
    /// <summary>
    /// The raw response content from the AI.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Indicates whether the AI call was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message if the call failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Number of tokens used in the prompt.
    /// </summary>
    public int? PromptTokens { get; }

    /// <summary>
    /// Number of tokens generated in the completion.
    /// </summary>
    public int? CompletionTokens { get; }

    /// <summary>
    /// Total tokens used (prompt + completion).
    /// </summary>
    public int? TotalTokens { get; }

    /// <summary>
    /// Time taken to complete the request.
    /// </summary>
    public TimeSpan ResponseTime { get; }

    /// <summary>
    /// Name of the model that generated the response.
    /// </summary>
    public string? ModelUsed { get; }

    private AIPromptResponse(
        string content,
        bool isSuccess,
        string? errorMessage,
        int? promptTokens,
        int? completionTokens,
        int? totalTokens,
        TimeSpan responseTime,
        string? modelUsed)
    {
        Content = content ?? string.Empty;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        PromptTokens = promptTokens;
        CompletionTokens = completionTokens;
        TotalTokens = totalTokens;
        ResponseTime = responseTime;
        ModelUsed = modelUsed;
    }

    /// <summary>
    /// Creates a successful AI response.
    /// </summary>
    public static AIPromptResponse Success(
        string content,
        int? promptTokens = null,
        int? completionTokens = null,
        int? totalTokens = null,
        TimeSpan? responseTime = null,
        string? modelUsed = null)
    {
        return new AIPromptResponse(
            content,
            isSuccess: true,
            errorMessage: null,
            promptTokens,
            completionTokens,
            totalTokens,
            responseTime ?? TimeSpan.Zero,
            modelUsed);
    }

    /// <summary>
    /// Creates a failed AI response.
    /// </summary>
    public static AIPromptResponse Failure(string errorMessage, TimeSpan? responseTime = null)
    {
        return new AIPromptResponse(
            content: string.Empty,
            isSuccess: false,
            errorMessage,
            promptTokens: null,
            completionTokens: null,
            totalTokens: null,
            responseTime ?? TimeSpan.Zero,
            modelUsed: null);
    }
}
