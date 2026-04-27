using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Contracts.Incidents;

namespace EventTicketingAiPlatform.Application.UseCases.Incidents
{
    public sealed class AssignIncidentHandler
    {
        private readonly IIncidentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignIncidentHandler(
            IIncidentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IncidentResponse?> HandleAsync(
            Guid id,
            AssignIncidentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.AssignedTo))
                throw new ArgumentException("AssignedTo is required.");

            var incident = await _repository.GetByIdAsync(id, cancellationToken);

            if (incident is null)
                return null;

            if (incident.Status == IncidentStatus.Resolved)
                throw new InvalidOperationException("Resolved incidents cannot be assigned.");

            incident.AssignedTo = request.AssignedTo.Trim();
            incident.AssignedAtUtc = DateTime.UtcNow;
            incident.Status = IncidentStatus.InProgress;

            await _repository.UpdateAsync(incident, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return IncidentMapper.ToResponse(incident);
        }
    }
}
