namespace EventTicketingAiPlatform.Application.Risk
{
    public sealed record RiskExplanationResult(
    string Summary,
    string OperatorMessage,
    string Confidence,
    string Provider);
}
