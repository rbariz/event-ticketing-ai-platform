using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Contracts.Incidents;

namespace EventTicketingAiPlatform.Application.UseCases.Incidents
{
    public sealed class ResolveIncidentHandler
    {
        private readonly IIncidentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ResolveIncidentHandler(
            IIncidentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IncidentResponse?> HandleAsync(
            Guid id,
            ResolveIncidentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.ResolutionNote))
                throw new ArgumentException("ResolutionNote is required.");

            var incident = await _repository.GetByIdAsync(id, cancellationToken);

            if (incident is null)
                return null;

            incident.Status = IncidentStatus.Resolved;
            incident.ResolvedAtUtc = DateTime.UtcNow;
            incident.ResolutionNote = request.ResolutionNote.Trim();

            await _repository.UpdateAsync(incident, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return IncidentMapper.ToResponse(incident);
        }
    }
}
