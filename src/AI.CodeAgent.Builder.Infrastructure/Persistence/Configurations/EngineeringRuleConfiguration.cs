using AI.CodeAgent.Builder.Domain.Entities;
using AI.CodeAgent.Builder.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for EngineeringRule entity.
/// Includes value object (RuleConstraint) configuration.
/// </summary>
public sealed class EngineeringRuleConfiguration : IEntityTypeConfiguration<EngineeringRule>
{
    public void Configure(EntityTypeBuilder<EngineeringRule> builder)
    {
        builder.ToTable("EngineeringRules");

        builder.HasKey(er => er.Id);

        builder.Property(er => er.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(er => er.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(er => er.Rationale)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(er => er.ImplementationGuidance)
            .HasMaxLength(5000);

        builder.Property(er => er.ExampleCode)
            .HasMaxLength(10000);

        builder.Property(er => er.IsEnforced)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(er => er.CreatedAt)
            .IsRequired();

        builder.Property(er => er.UpdatedAt)
            .IsRequired();

        // RuleConstraint value object - owned type
        builder.OwnsOne(er => er.Constraint, constraint =>
        {
            constraint.Property(c => c.Severity)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("Severity");

            constraint.Property(c => c.Scope)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("Scope");
        });

        // Indexes
        builder.HasIndex(er => er.Name)
            .IsUnique();

        builder.HasIndex(er => er.IsEnforced);
    }
}
