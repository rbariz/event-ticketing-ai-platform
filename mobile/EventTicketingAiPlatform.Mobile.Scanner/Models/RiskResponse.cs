namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class RiskResponse
    {
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = "";
        public string RiskExplanation { get; set; } = "";
        public List<string> RiskSignals { get; set; } = new();
        public string RecommendedAction { get; set; } = "";
        public string ExplanationSummary { get; set; } = "";
        public string ExplanationConfidence { get; set; } = "";
        public string ExplanationProvider { get; set; } = "";
    }
}
