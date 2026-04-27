using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Contracts.Incidents;

namespace EventTicketingAiPlatform.Application.UseCases.Incidents
{
    public sealed class GetIncidentByIdHandler
    {
        private readonly IIncidentRepository _repository;

        public GetIncidentByIdHandler(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IncidentResponse?> HandleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var incident = await _repository.GetByIdAsync(id, cancellationToken);

            return incident is null
                ? null
                : IncidentMapper.ToResponse(incident);
        }
    }
}
