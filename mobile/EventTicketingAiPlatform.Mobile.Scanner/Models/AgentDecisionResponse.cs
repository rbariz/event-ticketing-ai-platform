using EventTicketingAiPlatform.Contracts.Agent;

namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class AgentDecisionResponse
    {
        public string Severity { get; set; } = "";
        public List<string> Actions { get; set; } = [];
        public string Reason { get; set; } = "";
        public bool RequiresHumanReview { get; set; }

        public AgentEnrichmentResponse? Enrichment { get; set; }
    }

}
