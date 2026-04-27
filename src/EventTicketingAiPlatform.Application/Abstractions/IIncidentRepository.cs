using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface IIncidentRepository
    {
        Task AddAsync(
            Incident incident,
            CancellationToken cancellationToken = default);

        Task<Incident?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Incident?> GetOpenByScanAttemptIdAsync(
            Guid scanAttemptId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Incident>> SearchAsync(
            IncidentStatus? status,
            IncidentSeverity? severity,
            int count,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
    Incident incident,
    CancellationToken cancellationToken = default);
    }
}
