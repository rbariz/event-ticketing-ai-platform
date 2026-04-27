using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryAgentNotificationRepository : IAgentNotificationRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryAgentNotificationRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task AddAsync(AgentNotification n, CancellationToken ct)
        {
            _store.AgentNotifications.Add(n);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<AgentNotification>> GetAsync(
            bool unreadOnly,
            int count,
            CancellationToken ct)
        {
            var q = _store.AgentNotifications.AsEnumerable();

            if (unreadOnly)
                q = q.Where(x => !x.IsRead);

            return Task.FromResult<IReadOnlyList<AgentNotification>>(
                q.OrderByDescending(x => x.CreatedAtUtc)
                 .Take(count)
                 .ToList());
        }

        public Task MarkAsReadAsync(Guid id, CancellationToken ct)
        {
            var n = _store.AgentNotifications.FirstOrDefault(x => x.Id == id);
            if (n is not null)
            {
                n.IsRead = true;
                n.ReadAtUtc = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
    }
}
