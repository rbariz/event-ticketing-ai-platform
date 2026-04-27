namespace EventTicketingAiPlatform.Contracts.Agent
{
    public sealed record AgentEnrichmentResponse(
    string OperatorSummary,
    IReadOnlyList<string> SuggestedNextActions,
    decimal ConfidenceScore,
    string BusinessImpact,
    string Provider);
}
