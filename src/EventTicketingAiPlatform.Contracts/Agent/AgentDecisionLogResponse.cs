namespace EventTicketingAiPlatform.Contracts.Agent
{
    public sealed record AgentDecisionLogResponse(
    Guid Id,
    Guid ScanAttemptId,
    int RiskScore,
    string RiskLevel,
    string Severity,
    IReadOnlyList<string> Actions,
    string Reason,
    bool RequiresHumanReview,
    string Provider,
    DateTime CreatedAtUtc);
}
