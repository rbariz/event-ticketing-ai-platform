using EventTicketingAiPlatform.Application.Abstractions;

namespace EventTicketingAiPlatform.Infrastructure.Persistence
{
    public sealed class PgUnitOfWork : IUnitOfWork
    {
        private readonly EventTicketingDbContext _db;

        public PgUnitOfWork(EventTicketingDbContext db)
        {
            _db = db;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
