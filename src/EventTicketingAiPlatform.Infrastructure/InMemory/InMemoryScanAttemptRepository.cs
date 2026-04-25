using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryScanAttemptRepository : IScanAttemptRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryScanAttemptRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task AddAsync(
            ScanAttempt scanAttempt,
            CancellationToken cancellationToken = default)
        {
            _store.ScanAttempts.Add(scanAttempt);
            return Task.CompletedTask;
        }

        public Task<ScanAttempt?> GetRecentByTicketCodeAsync(
            string ticketCode,
            DateTime sinceUtc,
            CancellationToken cancellationToken = default)
        {
            var result = _store.ScanAttempts
                .Where(x => x.TicketCode == ticketCode && x.ScannedAtUtc >= sinceUtc)
                .OrderByDescending(x => x.ScannedAtUtc)
                .FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}
