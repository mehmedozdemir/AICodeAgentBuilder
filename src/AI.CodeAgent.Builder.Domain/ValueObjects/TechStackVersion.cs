using AI.CodeAgent.Builder.Domain.Common;

namespace AI.CodeAgent.Builder.Domain.ValueObjects;

/// <summary>
/// Value object representing a technology stack version.
/// Supports semantic versioning and basic version comparison.
/// </summary>
public sealed class TechStackVersion : ValueObject
{
    private TechStackVersion(string value)
    {
        Value = value;
        ParseVersion();
    }

    public string Value { get; }
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }

    public static TechStackVersion Create(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty.", nameof(version));

        return new TechStackVersion(version.Trim());
    }

    private void ParseVersion()
    {
        // Parse semantic version format: Major.Minor.Patch
        // Examples: 8.0, 3.11.0, 2024.1
        var parts = Value.Split('.');
        
        if (parts.Length >= 1 && int.TryParse(parts[0], out int major))
            Major = major;

        if (parts.Length >= 2 && int.TryParse(parts[1], out int minor))
            Minor = minor;

        if (parts.Length >= 3 && int.TryParse(parts[2], out int patch))
            Patch = patch;
    }

    /// <summary>
    /// Checks if this version is compatible with another version.
    /// Compatibility is defined as same major version.
    /// </summary>
    public bool IsCompatibleWith(TechStackVersion other)
    {
        return Major == other.Major;
    }

    /// <summary>
    /// Checks if this version is newer than another version.
    /// </summary>
    public bool IsNewerThan(TechStackVersion other)
    {
        if (Major != other.Major)
            return Major > other.Major;
        
        if (Minor != other.Minor)
            return Minor > other.Minor;
        
        return Patch > other.Patch;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
