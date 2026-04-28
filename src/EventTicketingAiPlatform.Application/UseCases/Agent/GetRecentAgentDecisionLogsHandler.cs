using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Contracts.Agent;

namespace EventTicketingAiPlatform.Application.UseCases.Agent
{
    public sealed class GetRecentAgentDecisionLogsHandler
    {
        private readonly IAgentDecisionLogRepository _repository;

        public GetRecentAgentDecisionLogsHandler(
            IAgentDecisionLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AgentDecisionLogResponse>> HandleAsync(
            int count = 20,
            CancellationToken cancellationToken = default)
        {
            if (count <= 0)
                count = 20;

            if (count > 200)
                count = 200;

            var logs = await _repository.GetRecentAsync(
                count,
                cancellationToken);

            return logs
                .Select(x => new AgentDecisionLogResponse(
                    x.Id,
                    x.ScanAttemptId,
                    x.RiskScore,
                    x.RiskLevel,
                    x.Severity,
                    SplitActions(x.Actions),
                    x.Reason,
                    x.RequiresHumanReview,
                    x.Provider,
                    x.CreatedAtUtc,
                    x.OperatorSummary,
                    SplitSuggestedActions(x.SuggestedNextActions),
                    x.ConfidenceScore,
                    x.BusinessImpact,
                    x.EnrichmentProvider))
                .ToList();
        }

        private static IReadOnlyList<string> SplitActions(string actions)
        {
            if (string.IsNullOrWhiteSpace(actions))
                return [];

            return actions
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        private static IReadOnlyList<string> SplitSuggestedActions(string? actions)
        {
            if (string.IsNullOrWhiteSpace(actions))
                return [];

            return actions
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
    }
}
