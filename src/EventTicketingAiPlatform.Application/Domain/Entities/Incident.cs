using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class Incident
    {
        public Guid Id { get; set; }

        public Guid ScanAttemptId { get; set; }

        public IncidentSeverity Severity { get; set; }

        public IncidentStatus Status { get; set; } = IncidentStatus.Open;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? AssignedTo { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? AssignedAtUtc { get; set; }

        public DateTime? ResolvedAtUtc { get; set; }

        public string? ResolutionNote { get; set; }
    }
}
