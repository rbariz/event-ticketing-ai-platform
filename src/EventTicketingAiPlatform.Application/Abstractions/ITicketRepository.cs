using EventTicketingAiPlatform.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByCodeAsync(
            string ticketCode,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Ticket ticket,
            CancellationToken cancellationToken = default);
    }
}
