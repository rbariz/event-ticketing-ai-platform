namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class AgentEnrichmentResponse
    {
        public string OperatorSummary { get; set; } = "";
        public List<string> SuggestedNextActions { get; set; } = [];
        public decimal ConfidenceScore { get; set; }
        public string BusinessImpact { get; set; } = "";
        public string Provider { get; set; } = "";
    }

}
