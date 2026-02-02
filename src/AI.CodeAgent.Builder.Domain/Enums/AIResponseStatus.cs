namespace AI.CodeAgent.Builder.Domain.Enums;

/// <summary>
/// Represents the validation status of an AI-generated response.
/// Used to track the lifecycle of AI outputs for audit and quality purposes.
/// </summary>
public enum AIResponseStatus
{
    /// <summary>
    /// Response received from AI but not yet validated.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Response has been validated and approved for use.
    /// </summary>
    Validated = 2,

    /// <summary>
    /// Response rejected due to validation failures.
    /// Should not be used in production configurations.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Response requires manual review.
    /// Used when automatic validation is inconclusive.
    /// </summary>
    RequiresReview = 4
}
