using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for TechStack entity.
/// Configures TechStack as aggregate root owning StackParameter collection.
/// </summary>
public sealed class TechStackConfiguration : IEntityTypeConfiguration<TechStack>
{
    public void Configure(EntityTypeBuilder<TechStack> builder)
    {
        builder.ToTable("TechStacks");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.CategoryId)
            .IsRequired();

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ts => ts.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ts => ts.DocumentationUrl)
            .HasMaxLength(500);

        builder.Property(ts => ts.CreatedAt)
            .IsRequired();

        builder.Property(ts => ts.UpdatedAt)
            .IsRequired();

        // DefaultVersion is a TechStackVersion value object (converted to string)
        builder.Property(ts => ts.DefaultVersion)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => v != null ? Domain.ValueObjects.TechStackVersion.Create(v) : null)
            .HasMaxLength(50);

        // Configure owned collection of StackParameters
        builder.HasMany(ts => ts.Parameters)
            .WithOne()
            .HasForeignKey("TechStackId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ts => ts.CategoryId);

        builder.HasIndex(ts => new { ts.CategoryId, ts.Name })
            .IsUnique();
    }
}
