using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryTicketRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task<Ticket?> GetByCodeAsync(
            string ticketCode,
            CancellationToken cancellationToken = default)
        {
            var ticket = _store.Tickets
                .FirstOrDefault(x => x.TicketCode == ticketCode);

            return Task.FromResult(ticket);
        }

        public Task UpdateAsync(
            Ticket ticket,
            CancellationToken cancellationToken = default)
        {
            var index = _store.Tickets.FindIndex(x => x.Id == ticket.Id);

            if (index >= 0)
                _store.Tickets[index] = ticket;

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync(
    CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Ticket>>(
                _store.Tickets.OrderBy(x => x.TicketCode).ToList());
        }
    }
}
