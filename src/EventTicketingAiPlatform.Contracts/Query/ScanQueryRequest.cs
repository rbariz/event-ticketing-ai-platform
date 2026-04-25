using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Query
{
    public sealed record ScanQueryRequest(
     DateTime? FromUtc,
     DateTime? ToUtc,
     string? GateId,
     string? Source,
     string? Decision,
     string? ReasonCode);

}
