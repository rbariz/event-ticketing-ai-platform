using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface IAgentNotificationRepository
    {
        Task AddAsync(
            AgentNotification notification,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AgentNotification>> GetAsync(
            bool unreadOnly,
            int count,
            CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
