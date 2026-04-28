using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Configurations
{
    public sealed partial class AgentDecisionLogConfiguration : IEntityTypeConfiguration<AgentDecisionLog>
    {
        public void Configure(EntityTypeBuilder<AgentDecisionLog> builder)
        {
            builder.ToTable("agent_decision_logs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ScanAttemptId).HasColumnName("scan_attempt_id").IsRequired();

            builder.Property(x => x.RiskScore).HasColumnName("risk_score").IsRequired();
            builder.Property(x => x.RiskLevel).HasColumnName("risk_level").HasMaxLength(50).IsRequired();

            builder.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Actions).HasColumnName("actions").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.RequiresHumanReview).HasColumnName("requires_human_review").IsRequired();

            builder.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(100).IsRequired();
            builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

            builder.HasIndex(x => x.ScanAttemptId);
            builder.HasIndex(x => x.CreatedAtUtc);
            builder.HasIndex(x => x.Severity);

            builder.Property(x => x.OperatorSummary)
    .HasColumnName("operator_summary")
    .HasMaxLength(2000);

            builder.Property(x => x.SuggestedNextActions)
                .HasColumnName("suggested_next_actions")
                .HasMaxLength(2000);

            builder.Property(x => x.ConfidenceScore)
                .HasColumnName("confidence_score")
                .HasPrecision(5, 2);

            builder.Property(x => x.BusinessImpact)
                .HasColumnName("business_impact")
                .HasMaxLength(2000);

            builder.Property(x => x.EnrichmentProvider)
                .HasColumnName("enrichment_provider")
                .HasMaxLength(100);
        }
}
}
