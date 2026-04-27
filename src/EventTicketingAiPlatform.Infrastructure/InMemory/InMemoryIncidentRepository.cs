using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryIncidentRepository : IIncidentRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryIncidentRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task AddAsync(
            Incident incident,
            CancellationToken cancellationToken = default)
        {
            _store.Incidents.Add(incident);
            return Task.CompletedTask;
        }

        public Task<Incident?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var incident = _store.Incidents.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(incident);
        }

        public Task<Incident?> GetOpenByScanAttemptIdAsync(
            Guid scanAttemptId,
            CancellationToken cancellationToken = default)
        {
            var incident = _store.Incidents.FirstOrDefault(
                x => x.ScanAttemptId == scanAttemptId &&
                     x.Status != IncidentStatus.Resolved);

            return Task.FromResult(incident);
        }

        public Task<IReadOnlyList<Incident>> SearchAsync(
            IncidentStatus? status,
            IncidentSeverity? severity,
            int count,
            CancellationToken cancellationToken = default)
        {
            var query = _store.Incidents.AsEnumerable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (severity.HasValue)
                query = query.Where(x => x.Severity == severity.Value);

            return Task.FromResult<IReadOnlyList<Incident>>(
                query
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Take(count)
                    .ToList());
        }
    }
}
