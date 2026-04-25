using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Scans
{
    public sealed record ScanAttemptResponse(
    Guid Id,
    string TicketCode,
    Guid? TicketId,
    string DeviceId,
    string GateId,
    DateTime ScannedAtUtc,
    string Decision,
    string ReasonCode,
    string? Source,
    long ProcessingTimeMs);
}
