using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryAgentDecisionLogRepository : IAgentDecisionLogRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryAgentDecisionLogRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task AddAsync(
            AgentDecisionLog log,
            CancellationToken cancellationToken = default)
        {
            _store.AgentDecisionLogs.Add(log);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<AgentDecisionLog>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<AgentDecisionLog>>(
                _store.AgentDecisionLogs
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Take(count)
                    .ToList());
        }
    }
}
