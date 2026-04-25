using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class ScanAttempt
    {
        public Guid Id { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public Guid? TicketId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string GateId { get; set; } = string.Empty;
        public DateTime ScannedAtUtc { get; set; }
        public ScanDecision Decision { get; set; }
        public ScanReasonCode ReasonCode { get; set; }

        public string? SourceIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Source { get; set; }
        public long ProcessingTimeMs { get; set; }
        public string? CorrelationId { get; set; }
    }
}
