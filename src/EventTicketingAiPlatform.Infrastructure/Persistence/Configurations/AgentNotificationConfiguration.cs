using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Configurations
{
    public sealed class AgentNotificationConfiguration : IEntityTypeConfiguration<AgentNotification>
    {
        public void Configure(EntityTypeBuilder<AgentNotification> builder)
        {
            builder.ToTable("agent_notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ScanAttemptId)
                .HasColumnName("scan_attempt_id")
                .IsRequired();

            builder.Property(x => x.Severity)
                .HasColumnName("severity")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Message)
                .HasColumnName("message")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .HasColumnName("is_read")
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.Property(x => x.ReadAtUtc)
                .HasColumnName("read_at_utc");

            builder.HasIndex(x => x.ScanAttemptId);
            builder.HasIndex(x => x.CreatedAtUtc);
            builder.HasIndex(x => x.IsRead);
        }
    }
}
