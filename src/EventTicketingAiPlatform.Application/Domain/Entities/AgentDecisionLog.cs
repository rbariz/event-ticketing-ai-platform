namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class AgentDecisionLog
    {
        public Guid Id { get; set; }

        public Guid ScanAttemptId { get; set; }

        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;
        public string Actions { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool RequiresHumanReview { get; set; }

        public string Provider { get; set; } = "RuleBasedAgent";

        public DateTime CreatedAtUtc { get; set; }

        public string? OperatorSummary { get; set; }
        public string? SuggestedNextActions { get; set; }
        public decimal? ConfidenceScore { get; set; }
        public string? BusinessImpact { get; set; }
        public string? EnrichmentProvider { get; set; }
    }
}
