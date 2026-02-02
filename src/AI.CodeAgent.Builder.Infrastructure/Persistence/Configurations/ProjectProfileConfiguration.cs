using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ProjectProfile entity (primary aggregate root).
/// Includes owned entities (ProfileTechStack) and collections.
/// </summary>
public sealed class ProjectProfileConfiguration : IEntityTypeConfiguration<ProjectProfile>
{
    public void Configure(EntityTypeBuilder<ProjectProfile> builder)
    {
        builder.ToTable("ProjectProfiles");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pp => pp.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(pp => pp.ProjectName)
            .HasMaxLength(200);

        builder.Property(pp => pp.TargetTeamSize);

        builder.Property(pp => pp.CreatedAt)
            .IsRequired();

        builder.Property(pp => pp.UpdatedAt)
            .IsRequired();

        // Configure owned collection of ProfileTechStack
        builder.OwnsMany(pp => pp.TechStacks, pts =>
        {
            pts.ToTable("ProfileTechStacks");

            pts.WithOwner().HasForeignKey("ProjectProfileId");

            pts.Property<Guid>("ProjectProfileId");

            pts.Property(p => p.TechStackId)
                .IsRequired();

            // ParameterValues dictionary stored as JSON
            pts.Property(p => p.ParameterValues)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(
                        v.ToDictionary(kvp => kvp.Key, kvp => new { Type = kvp.Value.Type.ToString(), Value = kvp.Value.Value }),
                        (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, ParameterValueDto>>(v, (System.Text.Json.JsonSerializerOptions?)null)!
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => CreateParameterValue(kvp.Value.Type, kvp.Value.Value)))
                .HasColumnType("TEXT");

            pts.HasKey("ProjectProfileId", nameof(ProfileTechStack.TechStackId));
        });

        // Architecture pattern IDs stored as JSON array
        builder.Property(pp => pp.ArchitecturePatternIds)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null)!)
            .HasColumnType("TEXT");

        // Engineering rule IDs stored as JSON array
        builder.Property(pp => pp.EngineeringRuleIds)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null)!)
            .HasColumnType("TEXT");

        // Indexes
        builder.HasIndex(pp => pp.ProjectName);
    }

    private static ParameterValue CreateParameterValue(string type, string value)
    {
        return type switch
        {
            "String" => ParameterValue.CreateString(value),
            "Number" => ParameterValue.CreateNumber(decimal.Parse(value)),
            "Boolean" => ParameterValue.CreateBoolean(bool.Parse(value)),
            "Enum" => ParameterValue.CreateEnum(value),
            "Version" => ParameterValue.CreateVersion(value),
            _ => ParameterValue.CreateString(value)
        };
    }
}

/// <summary>
/// DTO for JSON serialization of ParameterValue.
/// </summary>
internal sealed class ParameterValueDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
