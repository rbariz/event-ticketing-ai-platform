namespace EventTicketingAiPlatform.Application.Agent
{
    public sealed record AgentDecision(
    AgentSeverity Severity,
    IReadOnlyList<AgentActionType> Actions,
    string Reason,
    bool RequiresHumanReview);
}
