using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Configurations
{
    public sealed partial class AgentDecisionLogConfiguration
    {
        public sealed class IncidentConfiguration : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.ToTable("incidents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ScanAttemptId)
                .HasColumnName("scan_attempt_id")
                .IsRequired();

            builder.Property(x => x.Severity)
                .HasColumnName("severity")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.AssignedTo)
                .HasColumnName("assigned_to")
                .HasMaxLength(200);

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.Property(x => x.AssignedAtUtc)
                .HasColumnName("assigned_at_utc");

            builder.Property(x => x.ResolvedAtUtc)
                .HasColumnName("resolved_at_utc");

            builder.Property(x => x.ResolutionNote)
                .HasColumnName("resolution_note")
                .HasMaxLength(1000);

            builder.HasIndex(x => x.ScanAttemptId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Severity);
            builder.HasIndex(x => x.CreatedAtUtc);
        }
    }
}
}
