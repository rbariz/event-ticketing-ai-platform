using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Repositories
{
    public sealed class PgIncidentRepository : IIncidentRepository
    {
        private readonly EventTicketingDbContext _db;

        public PgIncidentRepository(EventTicketingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(
            Incident incident,
            CancellationToken cancellationToken = default)
        {
            await _db.Incidents.AddAsync(incident, cancellationToken);
        }

        public Task<Incident?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _db.Incidents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<Incident?> GetOpenByScanAttemptIdAsync(
            Guid scanAttemptId,
            CancellationToken cancellationToken = default)
        {
            return _db.Incidents.FirstOrDefaultAsync(
                x => x.ScanAttemptId == scanAttemptId &&
                     x.Status != IncidentStatus.Resolved,
                cancellationToken);
        }

        public async Task<IReadOnlyList<Incident>> SearchAsync(
            IncidentStatus? status,
            IncidentSeverity? severity,
            int count,
            CancellationToken cancellationToken = default)
        {
            var query = _db.Incidents.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (severity.HasValue)
                query = query.Where(x => x.Severity == severity.Value);

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
