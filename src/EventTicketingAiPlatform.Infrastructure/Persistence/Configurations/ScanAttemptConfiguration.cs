using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Configurations
{
    public sealed class ScanAttemptConfiguration : IEntityTypeConfiguration<ScanAttempt>
    {
        public void Configure(EntityTypeBuilder<ScanAttempt> builder)
        {
            builder.ToTable("scan_attempts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.TicketCode).HasColumnName("ticket_code").HasMaxLength(100).IsRequired();
            builder.Property(x => x.TicketId).HasColumnName("ticket_id");
            builder.Property(x => x.DeviceId).HasColumnName("device_id").HasMaxLength(100).IsRequired();
            builder.Property(x => x.GateId).HasColumnName("gate_id").HasMaxLength(100).IsRequired();
            builder.Property(x => x.ScannedAtUtc).HasColumnName("scanned_at_utc").IsRequired();
            builder.Property(x => x.Decision).HasColumnName("decision").IsRequired();
            builder.Property(x => x.ReasonCode).HasColumnName("reason_code").IsRequired();
            builder.Property(x => x.SourceIp).HasColumnName("source_ip").HasMaxLength(100);
            builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
            builder.Property(x => x.Source).HasColumnName("source").HasMaxLength(100);
            builder.Property(x => x.ProcessingTimeMs).HasColumnName("processing_time_ms").IsRequired();
            builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasMaxLength(100);

            builder.HasIndex(x => x.TicketCode);
            builder.HasIndex(x => x.ScannedAtUtc);
            builder.HasIndex(x => x.GateId);
            builder.HasIndex(x => x.Source);
            builder.HasIndex(x => new { x.TicketCode, x.ScannedAtUtc });
        }
    }
}
