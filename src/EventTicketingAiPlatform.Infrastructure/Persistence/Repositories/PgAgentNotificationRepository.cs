using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Repositories
{
    public sealed class PgAgentNotificationRepository : IAgentNotificationRepository
    {
        private readonly EventTicketingDbContext _db;

        public PgAgentNotificationRepository(EventTicketingDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(AgentNotification notification, CancellationToken ct)
        {
            await _db.AgentNotifications.AddAsync(notification, ct);
        }

        public async Task<IReadOnlyList<AgentNotification>> GetAsync(
            bool unreadOnly,
            int count,
            CancellationToken ct)
        {
            var query = _db.AgentNotifications.AsQueryable();

            if (unreadOnly)
                query = query.Where(x => !x.IsRead);

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(count)
                .ToListAsync(ct);
        }

        public async Task MarkAsReadAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.AgentNotifications.FindAsync([id], ct);
            if (entity is null)
                return;

            entity.IsRead = true;
            entity.ReadAtUtc = DateTime.UtcNow;
        }
    }
}
