namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class ValidateScanResponse
    {
        public bool Accepted { get; set; }
        public string Decision { get; set; } = "";
        public string ReasonCode { get; set; } = "";
        public string Message { get; set; } = "";
        public Guid ScanAttemptId { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = "";
        public string RecommendedAction { get; set; } = "";
        public List<string> RiskSignals { get; set; } = new();
    }
}
