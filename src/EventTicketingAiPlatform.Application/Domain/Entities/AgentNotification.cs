namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class AgentNotification
    {
        public Guid Id { get; set; }

        public Guid ScanAttemptId { get; set; }

        public string Severity { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? ReadAtUtc { get; set; }
    }
}
