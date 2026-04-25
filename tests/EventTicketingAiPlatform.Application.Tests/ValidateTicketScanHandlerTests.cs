using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Application.UseCases.ScanValidation;
using EventTicketingAiPlatform.Contracts.ScanValidation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Tests
{


    public sealed class ValidateTicketScanHandlerTests
    {
        [Fact]
        public async Task Should_accept_valid_ticket()
        {
            var now = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);
            var ticket = CreateTicket("TCK-001", now);

            var handler = CreateHandler(ticket);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("TCK-001", "DEV-1", "GATE-A", now, "unit-test"));

            response.Accepted.Should().BeTrue();
            response.Decision.Should().Be(nameof(ScanDecision.Accepted));
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.Ok));
            response.TicketId.Should().Be(ticket.Id);

            ticket.Status.Should().Be(TicketStatus.Consumed);
            ticket.ConsumedAtUtc.Should().Be(now);
        }

        [Fact]
        public async Task Should_reject_ticket_not_found()
        {
            var now = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);

            var handler = CreateHandler(ticket: null);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("UNKNOWN", "DEV-1", "GATE-A", now, "unit-test"));

            response.Accepted.Should().BeFalse();
            response.Decision.Should().Be(nameof(ScanDecision.Rejected));
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.TicketNotFound));
            response.TicketId.Should().BeNull();
        }

        [Fact]
        public async Task Should_reject_expired_ticket()
        {
            var now = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);

            var ticket = CreateTicket("EXP-001", now);
            ticket.ValidUntilUtc = now.AddMinutes(-1);

            var handler = CreateHandler(ticket);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("EXP-001", "DEV-1", "GATE-A", now, "unit-test"));

            response.Accepted.Should().BeFalse();
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.TicketExpired));
        }

        [Fact]
        public async Task Should_reject_already_used_ticket()
        {
            var now = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);

            var ticket = CreateTicket("USED-001", now);
            ticket.Status = TicketStatus.Consumed;
            ticket.ConsumedAtUtc = now.AddMinutes(-5);

            var handler = CreateHandler(ticket);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("USED-001", "DEV-1", "GATE-A", now, "unit-test"));

            response.Accepted.Should().BeFalse();
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.TicketAlreadyUsed));
        }

        [Fact]
        public async Task Should_reject_cancelled_ticket()
        {
            var now = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);

            var ticket = CreateTicket("CAN-001", now);
            ticket.Status = TicketStatus.Cancelled;

            var handler = CreateHandler(ticket);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("CAN-001", "DEV-1", "GATE-A", now, "unit-test"));

            response.Accepted.Should().BeFalse();
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.TicketCancelled));
        }

        [Fact]
        public async Task Should_reject_duplicate_scan()
        {
            var now = new DateTime(2026, 4, 25, 10, 1, 0, DateTimeKind.Utc);

            var ticket = CreateTicket("DUP-001", now);

            var existingAcceptedScan = new ScanAttempt
            {
                Id = Guid.NewGuid(),
                TicketCode = "DUP-001",
                TicketId = ticket.Id,
                DeviceId = "DEV-X",
                GateId = "GATE-A",
                ScannedAtUtc = now.AddSeconds(-1),
                Decision = ScanDecision.Accepted,
                ReasonCode = ScanReasonCode.Ok,
                Source = "unit-test"
            };

            var handler = CreateHandler(ticket, existingAcceptedScan);

            var response = await handler.HandleAsync(
                new ValidateTicketScanRequest("DUP-001", "DEV-Y", "GATE-B", now, "unit-test"));

            response.Accepted.Should().BeFalse();
            response.ReasonCode.Should().Be(nameof(ScanReasonCode.DuplicateScan));
        }

        private static Ticket CreateTicket(string code, DateTime now)
        {
            return new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                TicketCode = code,
                Status = TicketStatus.Active,
                ValidFromUtc = now.AddHours(-1),
                ValidUntilUtc = now.AddHours(2)
            };
        }

        private static ValidateTicketScanHandler CreateHandler(
            Ticket? ticket,
            ScanAttempt? existingScan = null)
        {
            var ticketRepo = new FakeTicketRepository(ticket);
            var scanRepo = new FakeScanAttemptRepository(existingScan);
            var uow = new FakeUnitOfWork();

            return new ValidateTicketScanHandler(
                ticketRepo,
                scanRepo,
                uow,
                new ValidateTicketScanRequestValidator(), new FakeRiskScoringService());
        }

        private sealed class FakeTicketRepository : ITicketRepository
        {
            private readonly Ticket? _ticket;

            public FakeTicketRepository(Ticket? ticket)
            {
                _ticket = ticket;
            }

            public Task<Ticket?> GetByCodeAsync(
                string ticketCode,
                CancellationToken cancellationToken = default)
            {
                if (_ticket is not null && _ticket.TicketCode == ticketCode)
                    return Task.FromResult<Ticket?>(_ticket);

                return Task.FromResult<Ticket?>(null);
            }

            public Task<IReadOnlyList<Ticket>> GetAllAsync(
    CancellationToken cancellationToken = default)
            {
                IReadOnlyList<Ticket> result = _ticket is null
                    ? []
                    : [_ticket];

                return Task.FromResult(result);
            }

            public Task UpdateAsync(
                Ticket ticket,
                CancellationToken cancellationToken = default)
                => Task.CompletedTask;
        }

        private sealed class FakeScanAttemptRepository : IScanAttemptRepository
        {
            private readonly List<ScanAttempt> _items = [];

            public FakeScanAttemptRepository(ScanAttempt? existingScan = null)
            {
                if (existingScan is not null)
                    _items.Add(existingScan);
            }

            public Task AddAsync(
                ScanAttempt scanAttempt,
                CancellationToken cancellationToken = default)
            {
                _items.Add(scanAttempt);
                return Task.CompletedTask;
            }
            public Task<IReadOnlyList<ScanAttempt>> GetAllAsync(
    CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyList<ScanAttempt>>(
                    _items.OrderByDescending(x => x.ScannedAtUtc).ToList());
            }

            public Task<ScanAttempt?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            {
                var result = _items.FirstOrDefault(x => x.Id == id);
                return Task.FromResult(result);
            }

            public Task<IReadOnlyList<ScanAttempt>> GetRecentAsync(
    int count,
    CancellationToken cancellationToken = default)
            {
                var result = _items
                    .OrderByDescending(x => x.ScannedAtUtc)
                    .Take(count)
                    .ToList();

                return Task.FromResult<IReadOnlyList<ScanAttempt>>(result);
            }

            public Task<ScanAttempt?> GetRecentByTicketCodeAsync(
                string ticketCode,
                DateTime sinceUtc,
                CancellationToken cancellationToken = default)
            {
                var result = _items
                    .Where(x => x.TicketCode == ticketCode && x.ScannedAtUtc >= sinceUtc)
                    .OrderByDescending(x => x.ScannedAtUtc)
                    .FirstOrDefault();

                return Task.FromResult(result);
            }
        }

        private sealed class FakeUnitOfWork : IUnitOfWork
        {
            public Task SaveChangesAsync(CancellationToken cancellationToken = default)
                => Task.CompletedTask;
        }

        private sealed class FakeRiskScoringService : IRiskScoringService
        {
            public AntifraudRiskAssessment Assess(
                Ticket? ticket,
                ScanAttempt attempt,
                ScanAttempt? recentScan)
            {
                return new AntifraudRiskAssessment
                {
                    RiskScore = 10,
                    RiskLevel = "Low",
                    RecommendedAction = "Allow",
                    RiskSignals = []
                };
            }
        }
    }
}
