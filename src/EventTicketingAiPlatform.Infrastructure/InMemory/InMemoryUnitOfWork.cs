using EventTicketingAiPlatform.Application.Abstractions;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryUnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
