using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for StackParameter entity.
/// StackParameter is owned by TechStack aggregate.
/// </summary>
public sealed class StackParameterConfiguration : IEntityTypeConfiguration<StackParameter>
{
    public void Configure(EntityTypeBuilder<StackParameter> builder)
    {
        builder.ToTable("StackParameters");

        builder.HasKey(sp => sp.Id);

        // Foreign key to TechStack (aggregate root)
        builder.Property<Guid>("TechStackId")
            .IsRequired();

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(sp => sp.ParameterType)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(sp => sp.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sp => sp.DefaultValue)
            .HasMaxLength(500);

        builder.Property(sp => sp.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // AllowedValues stored as JSON array
        builder.Property(sp => sp.AllowedValues)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex("TechStackId");

        builder.HasIndex("TechStackId", nameof(StackParameter.Name))
            .IsUnique();
    }
}
