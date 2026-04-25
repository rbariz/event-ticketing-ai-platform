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

        public Task<IReadOnlyList<ScanAttempt>> GetAllAsync(
    CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ScanAttempt>>(
                _store.ScanAttempts
                    .OrderByDescending(x => x.ScannedAtUtc)
                    .ToList());
        }

        public Task<ScanAttempt?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
        {
            var result = _store.ScanAttempts
                .FirstOrDefault(x => x.Id == id);

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<ScanAttempt>> GetRecentAsync(
    int count,
    CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ScanAttempt>>(
                _store.ScanAttempts
                    .OrderByDescending(x => x.ScannedAtUtc)
                    .Take(count)
                    .ToList());
        }

        public Task<IReadOnlyList<ScanAttempt>> SearchAsync(
    DateTime? fromUtc,
    DateTime? toUtc,
    string? gateId,
    string? source,
    string? decision,
    string? reasonCode,
    CancellationToken cancellationToken = default)
        {
            var query = _store.ScanAttempts.AsEnumerable();

            if (fromUtc.HasValue)
                query = query.Where(x => x.ScannedAtUtc >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(x => x.ScannedAtUtc <= toUtc.Value);

            if (!string.IsNullOrWhiteSpace(gateId))
                query = query.Where(x => x.GateId.Equals(gateId, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(x => string.Equals(x.Source, source, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(decision))
                query = query.Where(x => x.Decision.ToString().Equals(decision, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(reasonCode))
                query = query.Where(x => x.ReasonCode.ToString().Equals(reasonCode, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult<IReadOnlyList<ScanAttempt>>(
                query.OrderByDescending(x => x.ScannedAtUtc).ToList());
        }
    }
}
