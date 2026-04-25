namespace EventTicketingAiPlatform.Contracts.Risk
{
    public sealed record RiskAssessmentResponse(
    int RiskScore,
    string RiskLevel,
    string RiskExplanation,
    IReadOnlyList<string> RiskSignals,
    string RecommendedAction,
    string ExplanationSummary,
    string ExplanationConfidence,
    string ExplanationProvider);
}
