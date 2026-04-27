using EventTicketingAiPlatform.Contracts.Risk;

namespace EventTicketingAiPlatform.Application.Agent
{
    public interface IAgentEnrichmentService
    {
        Task<AgentEnrichment> EnrichAsync(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            string language,
            CancellationToken cancellationToken = default);
    }
}
