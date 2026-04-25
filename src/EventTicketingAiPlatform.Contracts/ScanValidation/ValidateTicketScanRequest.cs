using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.ScanValidation
{
    public sealed record ValidateTicketScanRequest(
    string TicketCode,
    string DeviceId,
    string GateId,
    DateTime ScannedAtUtc,
    string? Source = null);
}
