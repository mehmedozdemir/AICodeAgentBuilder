using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ArchitecturePattern entity.
/// </summary>
public sealed class ArchitecturePatternConfiguration : IEntityTypeConfiguration<ArchitecturePattern>
{
    public void Configure(EntityTypeBuilder<ArchitecturePattern> builder)
    {
        builder.ToTable("ArchitecturePatterns");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ap => ap.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ap => ap.ComplexityLevel)
            .IsRequired();

        builder.Property(ap => ap.SuitableForSmallTeams)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ap => ap.SuitableForLargeScale)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ap => ap.KeyPrinciples)
            .HasMaxLength(2000);

        builder.Property(ap => ap.AntiPatterns)
            .HasMaxLength(2000);

        builder.Property(ap => ap.IsAIGenerated)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ap => ap.CreatedAt)
            .IsRequired();

        builder.Property(ap => ap.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ap => ap.Name)
            .IsUnique();

        builder.HasIndex(ap => ap.ComplexityLevel);
    }
}
