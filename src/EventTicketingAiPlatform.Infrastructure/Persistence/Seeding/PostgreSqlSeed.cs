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
                },
                new Ticket
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    EventId = eventId,
                    TicketCode = "TCK-USED-001",
                    Status = TicketStatus.Consumed,
                    ValidFromUtc = now.AddDays(-1),
                    ValidUntilUtc = now.AddDays(30),
                    ConsumedAtUtc = now.AddMinutes(-20)
                },
                new Ticket
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    EventId = eventId,
                    TicketCode = "TCK-SUSPICIOUS-001",
                    Status = TicketStatus.Active,
                    ValidFromUtc = now.AddDays(-1),
                    ValidUntilUtc = now.AddDays(30)
                });

            db.ScanAttempts.AddRange(
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    TicketCode = "TCK-USED-001",
                    TicketId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    DeviceId = "DEV-001",
                    GateId = "GATE-A",
                    ScannedAtUtc = now.AddMinutes(-25),
                    Decision = ScanDecision.Accepted,
                    ReasonCode = ScanReasonCode.Ok,
                    Source = "seed",
                    ProcessingTimeMs = 12
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    TicketCode = "TCK-USED-001",
                    TicketId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    DeviceId = "DEV-002",
                    GateId = "GATE-B",
                    ScannedAtUtc = now.AddMinutes(-20),
                    Decision = ScanDecision.Rejected,
                    ReasonCode = ScanReasonCode.TicketAlreadyUsed,
                    Source = "seed",
                    ProcessingTimeMs = 9
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                    TicketCode = "TCK-EXPIRED-001",
                    TicketId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    DeviceId = "DEV-003",
                    GateId = "GATE-A",
                    ScannedAtUtc = now.AddMinutes(-18),
                    Decision = ScanDecision.Rejected,
                    ReasonCode = ScanReasonCode.TicketExpired,
                    Source = "seed",
                    ProcessingTimeMs = 10
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                    TicketCode = "TCK-CANCELLED-001",
                    TicketId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    DeviceId = "DEV-004",
                    GateId = "GATE-C",
                    ScannedAtUtc = now.AddMinutes(-15),
                    Decision = ScanDecision.Rejected,
                    ReasonCode = ScanReasonCode.TicketCancelled,
                    Source = "seed",
                    ProcessingTimeMs = 11
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"),
                    TicketCode = "TCK-SUSPICIOUS-001",
                    TicketId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    DeviceId = "DEV-010",
                    GateId = "GATE-A",
                    ScannedAtUtc = now.AddMinutes(-8),
                    Decision = ScanDecision.Accepted,
                    ReasonCode = ScanReasonCode.Ok,
                    Source = "seed",
                    ProcessingTimeMs = 14
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"),
                    TicketCode = "TCK-SUSPICIOUS-001",
                    TicketId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    DeviceId = "DEV-011",
                    GateId = "GATE-B",
                    ScannedAtUtc = now.AddMinutes(-7),
                    Decision = ScanDecision.Rejected,
                    ReasonCode = ScanReasonCode.DuplicateScan,
                    Source = "seed",
                    ProcessingTimeMs = 8
                },
                new ScanAttempt
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"),
                    TicketCode = "UNKNOWN-999",
                    TicketId = null,
                    DeviceId = "DEV-012",
                    GateId = "GATE-C",
                    ScannedAtUtc = now.AddMinutes(-5),
                    Decision = ScanDecision.Rejected,
                    ReasonCode = ScanReasonCode.TicketNotFound,
                    Source = "seed",
                    ProcessingTimeMs = 7
                });

            await db.SaveChangesAsync();
        }
    }
}
