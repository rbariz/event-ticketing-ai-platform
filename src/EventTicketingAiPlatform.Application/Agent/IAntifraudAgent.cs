using EventTicketingAiPlatform.Contracts.Risk;

namespace EventTicketingAiPlatform.Application.Agent
{
    public interface IAntifraudAgent
    {
        Task<AgentDecision> AnalyzeAsync(
            RiskAssessmentResponse risk,
            CancellationToken cancellationToken = default);
    }
}
