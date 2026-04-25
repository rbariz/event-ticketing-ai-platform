using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class Ticket
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidUntilUtc { get; set; }
        public DateTime? ConsumedAtUtc { get; set; }

        public bool IsExpired(DateTime scannedAtUtc)
            => scannedAtUtc > ValidUntilUtc;

        public bool IsValidAt(DateTime scannedAtUtc)
            => scannedAtUtc >= ValidFromUtc && scannedAtUtc <= ValidUntilUtc;

        public bool IsAlreadyUsed()
            => Status == TicketStatus.Consumed || ConsumedAtUtc is not null;

        public void MarkAsConsumed(DateTime consumedAtUtc)
        {
            Status = TicketStatus.Consumed;
            ConsumedAtUtc = consumedAtUtc;
        }
    }
}
