using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Repositories
{
    public sealed class PgTicketRepository : ITicketRepository
    {
        private readonly EventTicketingDbContext _db;

        public PgTicketRepository(EventTicketingDbContext db)
        {
            _db = db;
        }

        public Task<Ticket?> GetByCodeAsync(string ticketCode, CancellationToken cancellationToken = default)
        {
            return _db.Tickets.FirstOrDefaultAsync(x => x.TicketCode == ticketCode, cancellationToken);
        }

        public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Tickets.OrderBy(x => x.TicketCode).ToListAsync(cancellationToken);
        }

        public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            _db.Tickets.Update(ticket);
            return Task.CompletedTask;
        }
    }
}
