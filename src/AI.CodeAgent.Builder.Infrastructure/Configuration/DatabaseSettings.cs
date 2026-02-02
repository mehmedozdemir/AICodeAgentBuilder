namespace AI.CodeAgent.Builder.Infrastructure.Configuration;

/// <summary>
/// Database configuration settings.
/// SQLite is used for simplicity and portability (single-file database).
/// </summary>
public sealed class DatabaseSettings
{
    public const string SectionName = "Database";

    /// <summary>
    /// SQLite connection string.
    /// Default: Data Source=aiagent.db
    /// </summary>
    public string ConnectionString { get; set; } = "Data Source=aiagent.db";

    /// <summary>
    /// Enable detailed logging for EF Core queries.
    /// Should be false in production.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Enable detailed query logging.
    /// Should be false in production for performance.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
}
