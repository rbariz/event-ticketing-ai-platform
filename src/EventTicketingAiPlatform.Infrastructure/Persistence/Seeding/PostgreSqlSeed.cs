using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Seeding
{
    public static class PostgreSqlSeed
    {
        public static async Task SeedAsync(EventTicketingDbContext db)
        {
            if (await db.Events.AnyAsync())
                return;

            var now = DateTime.UtcNow;
            var eventId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            db.Events.Add(new Event
            {
                Id = eventId,
                Name = "AI Antifraud Demo Event",
                StartsAtUtc = now.AddDays(-1),
                EndsAtUtc = now.AddDays(30)
            });

            db.Tickets.AddRange(
                new Ticket
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    EventId = eventId,
                    TicketCode = "TCK-VALID-001",
                    Status = TicketStatus.Active,
                    ValidFromUtc = now.AddDays(-1),
                    ValidUntilUtc = now.AddDays(30)
                },
                new Ticket
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    EventId = eventId,
                    TicketCode = "TCK-EXPIRED-001",
                    Status = TicketStatus.Active,
                    ValidFromUtc = now.AddDays(-10),
                    ValidUntilUtc = now.AddDays(-1)
                },
                new Ticket
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    EventId = eventId,
                    TicketCode = "TCK-CANCELLED-001",
                    Status = TicketStatus.Cancelled,
                    ValidFromUtc = now.AddDays(-1),
                    ValidUntilUtc = now.AddDays(30)
                });

            await db.SaveChangesAsync();
        }
    }
}
