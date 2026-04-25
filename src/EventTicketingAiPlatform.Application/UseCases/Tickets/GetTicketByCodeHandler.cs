using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Contracts.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.Tickets
{
    public sealed class GetTicketByCodeHandler
    {
        private readonly ITicketRepository _repository;

        public GetTicketByCodeHandler(ITicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<TicketDetailsResponse?> HandleAsync(
            string ticketCode,
            CancellationToken cancellationToken = default)
        {
            var ticket = await _repository.GetByCodeAsync(ticketCode, cancellationToken);

            if (ticket is null)
                return null;

            return new TicketDetailsResponse(
                ticket.Id,
                ticket.EventId,
                ticket.TicketCode,
                ticket.Status.ToString(),
                ticket.ValidFromUtc,
                ticket.ValidUntilUtc,
                ticket.ConsumedAtUtc);
        }
    }
}
