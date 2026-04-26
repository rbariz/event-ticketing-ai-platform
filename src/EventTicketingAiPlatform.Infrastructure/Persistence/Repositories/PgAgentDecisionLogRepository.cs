using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Repositories
{
    public sealed class PgAgentDecisionLogRepository : IAgentDecisionLogRepository
    {
        private readonly EventTicketingDbContext _db;

        public PgAgentDecisionLogRepository(EventTicketingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(
            AgentDecisionLog log,
            CancellationToken cancellationToken = default)
        {
            await _db.AgentDecisionLogs.AddAsync(log, cancellationToken);
        }

        public async Task<IReadOnlyList<AgentDecisionLog>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            return await _db.AgentDecisionLogs
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
