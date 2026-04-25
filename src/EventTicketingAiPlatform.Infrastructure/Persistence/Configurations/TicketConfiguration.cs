using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Configurations
{
    public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).HasColumnName("event_id").IsRequired();

            builder.Property(x => x.TicketCode)
                .HasColumnName("ticket_code")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(x => x.ValidFromUtc).HasColumnName("valid_from_utc").IsRequired();
            builder.Property(x => x.ValidUntilUtc).HasColumnName("valid_until_utc").IsRequired();
            builder.Property(x => x.ConsumedAtUtc).HasColumnName("consumed_at_utc");

            builder.HasIndex(x => x.TicketCode).IsUnique();
            builder.HasIndex(x => x.EventId);
        }
    }
}
