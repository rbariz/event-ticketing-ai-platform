using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public static class InMemorySeed
    {
        public static void Seed(InMemoryStore store)
        {
            if (store.Tickets.Count > 0)
                return;

            var now = DateTime.UtcNow;
            var eventId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            store.Tickets.AddRange(
            [
                new Ticket
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                EventId = eventId,
                TicketCode = "TCK-VALID-001",
                Status = TicketStatus.Active,
                ValidFromUtc = now.AddHours(-1),
                ValidUntilUtc = now.AddHours(30)
            },
            new Ticket
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                EventId = eventId,
                TicketCode = "TCK-EXPIRED-001",
                Status = TicketStatus.Active,
                ValidFromUtc = now.AddHours(-4),
                ValidUntilUtc = now.AddHours(-1)
            },
            new Ticket
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                EventId = eventId,
                TicketCode = "TCK-CANCELLED-001",
                Status = TicketStatus.Cancelled,
                ValidFromUtc = now.AddHours(-1),
                ValidUntilUtc = now.AddHours(4)
            }
            ]);
        }
    }
}
