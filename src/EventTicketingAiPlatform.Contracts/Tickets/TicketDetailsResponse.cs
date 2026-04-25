using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Tickets
{
    public sealed record TicketDetailsResponse(
    Guid Id,
    Guid EventId,
    string TicketCode,
    string Status,
    DateTime ValidFromUtc,
    DateTime ValidUntilUtc,
    DateTime? ConsumedAtUtc);
}
