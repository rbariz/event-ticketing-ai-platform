using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Contracts.Agent;

namespace EventTicketingAiPlatform.Application.UseCases.Agent
{
    public sealed class GetAgentNotificationsHandler
    {
        private readonly IAgentNotificationRepository _repository;

        public GetAgentNotificationsHandler(IAgentNotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AgentNotificationResponse>> HandleAsync(
            bool unreadOnly = false,
            int count = 20,
            CancellationToken cancellationToken = default)
        {
            if (count <= 0)
                count = 20;

            if (count > 200)
                count = 200;

            var items = await _repository.GetAsync(
                unreadOnly,
                count,
                cancellationToken);

            return items
                .Select(x => new AgentNotificationResponse(
                    x.Id,
                    x.ScanAttemptId,
                    x.Severity,
                    x.Title,
                    x.Message,
                    x.IsRead,
                    x.CreatedAtUtc,
                    x.ReadAtUtc))
                .ToList();
        }
    }
}
