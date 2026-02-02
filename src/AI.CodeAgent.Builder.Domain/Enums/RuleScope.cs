namespace AI.CodeAgent.Builder.Domain.Enums;

/// <summary>
/// Defines the scope where an engineering rule applies.
/// Enables context-specific rule enforcement in generated code agent instructions.
/// </summary>
public enum RuleScope
{
    /// <summary>
    /// Rule applies globally to the entire project.
    /// Examples: code coverage minimum, licensing requirements.
    /// </summary>
    Global = 1,

    /// <summary>
    /// Rule applies to backend/server-side code only.
    /// Examples: no dynamic SQL, use repository pattern.
    /// </summary>
    Backend = 2,

    /// <summary>
    /// Rule applies to frontend/client-side code only.
    /// Examples: accessibility standards, responsive design.
    /// </summary>
    Frontend = 3,

    /// <summary>
    /// Rule applies to database and data access layer.
    /// Examples: migration strategy, no direct queries in business logic.
    /// </summary>
    Database = 4,

    /// <summary>
    /// Rule applies to testing practices.
    /// Examples: test naming conventions, mocking standards.
    /// </summary>
    Testing = 5,

    /// <summary>
    /// Rule applies to security concerns.
    /// Examples: authentication requirements, encryption standards.
    /// </summary>
    Security = 6,

    /// <summary>
    /// Rule applies to DevOps and deployment.
    /// Examples: container standards, CI/CD requirements.
    /// </summary>
    DevOps = 7
}
