namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class IncidentResponse
    {
        public Guid Id { get; set; }
        public Guid ScanAttemptId { get; set; }
        public string Severity { get; set; } = "";
        public string Status { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string? AssignedTo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? AssignedAtUtc { get; set; }
        public DateTime? ResolvedAtUtc { get; set; }
        public string? ResolutionNote { get; set; }
    }

}
