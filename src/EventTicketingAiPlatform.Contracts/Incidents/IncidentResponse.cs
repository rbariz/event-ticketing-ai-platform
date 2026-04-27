using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Incidents
{
    public sealed record IncidentResponse(
    Guid Id,
    Guid ScanAttemptId,
    string Severity,
    string Status,
    string Title,
    string Description,
    string? AssignedTo,
    DateTime CreatedAtUtc,
    DateTime? AssignedAtUtc,
    DateTime? ResolvedAtUtc,
    string? ResolutionNote);
}
