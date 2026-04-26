using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface IAgentDecisionLogRepository
    {
        Task AddAsync(
            AgentDecisionLog log,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AgentDecisionLog>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default);
    }
}
