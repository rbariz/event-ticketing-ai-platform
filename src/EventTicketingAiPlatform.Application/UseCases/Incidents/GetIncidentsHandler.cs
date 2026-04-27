using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Contracts.Incidents;

namespace EventTicketingAiPlatform.Application.UseCases.Incidents
{
    public sealed class GetIncidentsHandler
    {
        private readonly IIncidentRepository _repository;

        public GetIncidentsHandler(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<IncidentResponse>> HandleAsync(
            string? status,
            string? severity,
            int count = 50,
            CancellationToken cancellationToken = default)
        {
            if (count <= 0)
                count = 50;

            if (count > 200)
                count = 200;

            IncidentStatus? parsedStatus = null;
            IncidentSeverity? parsedSeverity = null;

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<IncidentStatus>(status, ignoreCase: true, out var s))
            {
                parsedStatus = s;
            }

            if (!string.IsNullOrWhiteSpace(severity) &&
                Enum.TryParse<IncidentSeverity>(severity, ignoreCase: true, out var sev))
            {
                parsedSeverity = sev;
            }

            var incidents = await _repository.SearchAsync(
                parsedStatus,
                parsedSeverity,
                count,
                cancellationToken);

            return incidents
                .Select(IncidentMapper.ToResponse)
                .ToList();
        }
    }
}
