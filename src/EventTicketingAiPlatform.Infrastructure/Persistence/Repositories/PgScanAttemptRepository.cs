using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Repositories
{
    public sealed class PgScanAttemptRepository : IScanAttemptRepository
    {
        private readonly EventTicketingDbContext _db;

        public PgScanAttemptRepository(EventTicketingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ScanAttempt scanAttempt, CancellationToken cancellationToken = default)
        {
            await _db.ScanAttempts.AddAsync(scanAttempt, cancellationToken);
        }

        public Task<ScanAttempt?> GetRecentByTicketCodeAsync(
            string ticketCode,
            DateTime sinceUtc,
            CancellationToken cancellationToken = default)
        {
            return _db.ScanAttempts
                .Where(x => x.TicketCode == ticketCode && x.ScannedAtUtc >= sinceUtc)
                .OrderByDescending(x => x.ScannedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ScanAttempt>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ScanAttempts
                .OrderByDescending(x => x.ScannedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public Task<ScanAttempt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _db.ScanAttempts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ScanAttempt>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _db.ScanAttempts
                .OrderByDescending(x => x.ScannedAtUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ScanAttempt>> SearchAsync(
            DateTime? fromUtc,
            DateTime? toUtc,
            string? gateId,
            string? source,
            string? decision,
            string? reasonCode,
            CancellationToken cancellationToken = default)
        {
            var query = _db.ScanAttempts.AsQueryable();

            if (fromUtc.HasValue)
                query = query.Where(x => x.ScannedAtUtc >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(x => x.ScannedAtUtc <= toUtc.Value);

            if (!string.IsNullOrWhiteSpace(gateId))
                query = query.Where(x => x.GateId == gateId);

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(x => x.Source == source);

            if (!string.IsNullOrWhiteSpace(decision))
                query = query.Where(x => x.Decision.ToString() == decision);

            if (!string.IsNullOrWhiteSpace(reasonCode))
                query = query.Where(x => x.ReasonCode.ToString() == reasonCode);

            return await query
                .OrderByDescending(x => x.ScannedAtUtc)
                .ToListAsync(cancellationToken);
        }
    }
}
