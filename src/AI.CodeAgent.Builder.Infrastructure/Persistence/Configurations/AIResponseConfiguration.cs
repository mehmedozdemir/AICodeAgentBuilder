using AI.CodeAgent.Builder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for AIResponse entity (audit trail).
/// Tracks all AI interactions for traceability and analysis.
/// </summary>
public sealed class AIResponseConfiguration : IEntityTypeConfiguration<AIResponse>
{
    public void Configure(EntityTypeBuilder<AIResponse> builder)
    {
        builder.ToTable("AIResponses");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Prompt)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(ar => ar.RawResponse)
            .IsRequired()
            .HasMaxLength(50000);

        builder.Property(ar => ar.RequestContext)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ar => ar.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ar => ar.ValidatedBy)
            .HasMaxLength(200);

        builder.Property(ar => ar.ValidationErrors)
            .HasMaxLength(2000);

        builder.Property(ar => ar.TokenCount);

        builder.Property(ar => ar.ResponseTimeMs);

        builder.Property(ar => ar.CreatedAt)
            .IsRequired();

        builder.Property(ar => ar.ValidatedAt);

        // Indexes for querying
        builder.HasIndex(ar => ar.Status);

        builder.HasIndex(ar => ar.RequestContext);

        builder.HasIndex(ar => ar.CreatedAt);
    }
}
