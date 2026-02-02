using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Domain.Enums;

namespace AI.CodeAgent.Builder.Domain.Entities;

/// <summary>
/// Represents an audit record of AI-generated content.
/// Stores both raw and processed AI outputs for traceability, validation, and quality control.
/// 
/// This entity is critical for:
/// - Tracking AI interaction history
/// - Validating AI-generated content before use
/// - Auditing and improving AI prompt engineering
/// - Compliance and quality assurance
/// </summary>
public sealed class AIResponse : BaseEntity
{
    private string _prompt = string.Empty;
    private string _rawResponse = string.Empty;

    // Private constructor for EF Core
    private AIResponse()
    {
        RequestContext = string.Empty;
    }

    private AIResponse(string prompt, string rawResponse, string requestContext)
    {
        SetPrompt(prompt);
        SetRawResponse(rawResponse);
        RequestContext = requestContext;
        Status = AIResponseStatus.Pending;
        RequestedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The prompt sent to the AI.
    /// Stored for reproducibility and prompt engineering improvement.
    /// </summary>
    public string Prompt
    {
        get => _prompt;
        private set => _prompt = value;
    }

    /// <summary>
    /// The raw, unprocessed response from the AI.
    /// Preserved for audit and debugging purposes.
    /// </summary>
    public string RawResponse
    {
        get => _rawResponse;
        private set => _rawResponse = value;
    }

    /// <summary>
    /// Processed/parsed content extracted from the raw response.
    /// This is the actual data used in the system after validation.
    /// </summary>
    public string? ProcessedContent { get; private set; }

    /// <summary>
    /// Context information about what triggered this AI request.
    /// Examples: "CategoryGeneration", "TechStackResearch", "RuleCreation".
    /// </summary>
    public string RequestContext { get; private set; }

    /// <summary>
    /// The validation status of this AI response.
    /// </summary>
    public AIResponseStatus Status { get; private set; }

    /// <summary>
    /// When the AI request was made.
    /// </summary>
    public DateTime RequestedAt { get; private set; }

    /// <summary>
    /// When the response was validated (if applicable).
    /// </summary>
    public DateTime? ValidatedAt { get; private set; }

    /// <summary>
    /// Who or what validated this response.
    /// Could be a user ID, system component, or automated validator.
    /// </summary>
    public string? ValidatedBy { get; private set; }

    /// <summary>
    /// Validation errors or rejection reasons (if Status is Rejected).
    /// Stored as JSON or delimited string.
    /// </summary>
    public string? ValidationErrors { get; private set; }

    /// <summary>
    /// AI model or version used for this request.
    /// Examples: "gpt-4", "claude-3", "custom-model-v1".
    /// </summary>
    public string? AIModel { get; private set; }

    /// <summary>
    /// Approximate token count for the request.
    /// Used for cost tracking and optimization.
    /// </summary>
    public int? TokenCount { get; private set; }

    /// <summary>
    /// Response time in milliseconds.
    /// Used for performance monitoring.
    /// </summary>
    public int? ResponseTimeMs { get; private set; }

    /// <summary>
    /// Additional metadata as JSON.
    /// Can store model parameters, temperature, etc.
    /// </summary>
    public string? Metadata { get; private set; }

    /// <summary>
    /// Factory method to create a new AI response record.
    /// </summary>
    public static AIResponse Create(
        string prompt,
        string rawResponse,
        string requestContext,
        string? aiModel = null)
    {
        var response = new AIResponse(prompt, rawResponse, requestContext)
        {
            AIModel = aiModel
        };

        return response;
    }

    /// <summary>
    /// Updates the prompt (for historical correction only).
    /// </summary>
    private void SetPrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));

        _prompt = prompt;
    }

    /// <summary>
    /// Updates the raw response (for historical correction only).
    /// </summary>
    private void SetRawResponse(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
            throw new ArgumentException("Raw response cannot be empty.", nameof(rawResponse));

        _rawResponse = rawResponse;
    }

    /// <summary>
    /// Sets the processed content after parsing and validation.
    /// </summary>
    public void SetProcessedContent(string processedContent)
    {
        if (string.IsNullOrWhiteSpace(processedContent))
            throw new ArgumentException("Processed content cannot be empty.", nameof(processedContent));

        ProcessedContent = processedContent;
        SetUpdatedAt();
    }

    /// <summary>
    /// Marks the response as validated and approved.
    /// </summary>
    public void MarkAsValidated(string validatedBy)
    {
        if (string.IsNullOrWhiteSpace(validatedBy))
            throw new ArgumentException("Validator identifier cannot be empty.", nameof(validatedBy));

        Status = AIResponseStatus.Validated;
        ValidatedAt = DateTime.UtcNow;
        ValidatedBy = validatedBy;
        ValidationErrors = null;
        SetUpdatedAt();
    }

    /// <summary>
    /// Marks the response as rejected with validation errors.
    /// </summary>
    public void MarkAsRejected(string validationErrors, string validatedBy)
    {
        if (string.IsNullOrWhiteSpace(validationErrors))
            throw new ArgumentException("Validation errors must be provided for rejection.", nameof(validationErrors));

        if (string.IsNullOrWhiteSpace(validatedBy))
            throw new ArgumentException("Validator identifier cannot be empty.", nameof(validatedBy));

        Status = AIResponseStatus.Rejected;
        ValidatedAt = DateTime.UtcNow;
        ValidatedBy = validatedBy;
        ValidationErrors = validationErrors;
        SetUpdatedAt();
    }

    /// <summary>
    /// Marks the response as requiring manual review.
    /// </summary>
    public void MarkAsRequiresReview(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason for review must be provided.", nameof(reason));

        Status = AIResponseStatus.RequiresReview;
        ValidationErrors = reason;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets performance metrics for this AI interaction.
    /// </summary>
    public void SetPerformanceMetrics(int? tokenCount, int? responseTimeMs)
    {
        if (tokenCount.HasValue && tokenCount.Value < 0)
            throw new ArgumentException("Token count cannot be negative.", nameof(tokenCount));

        if (responseTimeMs.HasValue && responseTimeMs.Value < 0)
            throw new ArgumentException("Response time cannot be negative.", nameof(responseTimeMs));

        TokenCount = tokenCount;
        ResponseTimeMs = responseTimeMs;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets additional metadata for this AI interaction.
    /// </summary>
    public void SetMetadata(string? metadata)
    {
        Metadata = metadata;
        SetUpdatedAt();
    }

    /// <summary>
    /// Checks if the response is usable (validated and not rejected).
    /// </summary>
    public bool IsUsable()
    {
        return Status == AIResponseStatus.Validated;
    }

    /// <summary>
    /// Checks if the response needs attention (pending or requires review).
    /// </summary>
    public bool NeedsAttention()
    {
        return Status == AIResponseStatus.Pending || Status == AIResponseStatus.RequiresReview;
    }
}
