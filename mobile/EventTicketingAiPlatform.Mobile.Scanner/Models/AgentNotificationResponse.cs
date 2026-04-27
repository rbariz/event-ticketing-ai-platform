namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class AgentNotificationResponse
    {
        public Guid Id { get; set; }
        public Guid ScanAttemptId { get; set; }
        public string Severity { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
    }

}
