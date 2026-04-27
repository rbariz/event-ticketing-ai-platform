namespace EventTicketingAiPlatform.Application.Agent
{
    public sealed record AgentEnrichment(
    string OperatorSummary,
    IReadOnlyList<string> SuggestedNextActions,
    decimal ConfidenceScore,
    string BusinessImpact,
    string Provider);
}
