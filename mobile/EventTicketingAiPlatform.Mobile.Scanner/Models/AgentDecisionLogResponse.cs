namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class AgentDecisionLogResponse
    {
        public Guid Id { get; set; }
        public Guid ScanAttemptId { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = "";
        public string Severity { get; set; } = "";
        public List<string> Actions { get; set; } = [];
        public string Reason { get; set; } = "";
        public bool RequiresHumanReview { get; set; }
        public string Provider { get; set; } = "";
        public DateTime CreatedAtUtc { get; set; }

        public string? OperatorSummary { get; set; }
        public List<string> SuggestedNextActions { get; set; } = [];
        public decimal? ConfidenceScore { get; set; }
        public string? BusinessImpact { get; set; }
        public string? EnrichmentProvider { get; set; }
    }

}
