using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Contracts.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.Incidents
{

    internal static class IncidentMapper
    {
        public static IncidentResponse ToResponse(Incident incident)
        {
            return new IncidentResponse(
                incident.Id,
                incident.ScanAttemptId,
                incident.Severity.ToString(),
                incident.Status.ToString(),
                incident.Title,
                incident.Description,
                incident.AssignedTo,
                incident.CreatedAtUtc,
                incident.AssignedAtUtc,
                incident.ResolvedAtUtc,
                incident.ResolutionNote);
        }
    }
}
