using EventTicketingAiPlatform.Contracts.Risk;

namespace EventTicketingAiPlatform.Application.Agent
{
    public sealed class RuleBasedAntifraudAgent : IAntifraudAgent
    {
        public Task<AgentDecision> AnalyzeAsync(
            RiskAssessmentResponse risk,
            CancellationToken cancellationToken = default)
        {
            var signals = risk.RiskSignals
                .Select(x => x.ToLowerInvariant())
                .ToHashSet();

            if (risk.RiskLevel == "High" &&
                (signals.Contains("duplicate_scan") ||
                 signals.Contains("multi_device") ||
                 signals.Contains("unknown_ticket")))
            {
                return Task.FromResult(new AgentDecision(
                    Severity: AgentSeverity.High,
                    Actions:
                    [
                        AgentActionType.CreateIncident,
                    AgentActionType.NotifyOps,
                    AgentActionType.RequireManualReview
                    ],
                    Reason: "High-risk antifraud pattern detected. An incident should be created and reviewed by an operator.",
                    RequiresHumanReview: true));
            }

            if (risk.RiskLevel == "Medium")
            {
                return Task.FromResult(new AgentDecision(
                    Severity: AgentSeverity.Medium,
                    Actions:
                    [
                        AgentActionType.Monitor,
                    AgentActionType.RequireManualReview
                    ],
                    Reason: "Moderate risk signals detected. Monitoring and manual review are recommended.",
                    RequiresHumanReview: true));
            }

            if (risk.RecommendedAction == "ManualReview")
            {
                return Task.FromResult(new AgentDecision(
                    Severity: AgentSeverity.Medium,
                    Actions:
                    [
                        AgentActionType.RequireManualReview
                    ],
                    Reason: "The risk engine recommends manual review.",
                    RequiresHumanReview: true));
            }

            return Task.FromResult(new AgentDecision(
                Severity: AgentSeverity.Info,
                Actions:
                [
                    AgentActionType.NoAction
                ],
                Reason: "No agent action required.",
                RequiresHumanReview: false));
        }
    }
}
