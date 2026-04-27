using EventTicketingAiPlatform.Application.Abstractions;

namespace EventTicketingAiPlatform.Application.UseCases.Agent
{
    public sealed class MarkAgentNotificationAsReadHandler
    {
        private readonly IAgentNotificationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAgentNotificationAsReadHandler(
            IAgentNotificationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await _repository.MarkAsReadAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
