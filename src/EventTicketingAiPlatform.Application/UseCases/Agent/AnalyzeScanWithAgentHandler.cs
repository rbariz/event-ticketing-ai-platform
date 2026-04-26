using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTicketingAiPlatform.Application.Agent;
using EventTicketingAiPlatform.Application.UseCases.Risk;
using EventTicketingAiPlatform.Contracts.Agent;

namespace EventTicketingAiPlatform.Application.UseCases.Agent
{

    


    public sealed class AnalyzeScanWithAgentHandler
    {
        private readonly GetScanRiskAssessmentHandler _riskHandler;
        private readonly IAntifraudAgent _agent;

        public AnalyzeScanWithAgentHandler(
            GetScanRiskAssessmentHandler riskHandler,
            IAntifraudAgent agent)
        {
            _riskHandler = riskHandler;
            _agent = agent;
        }

        public async Task<AgentDecisionResponse?> HandleAsync(
            Guid scanId,
            string language = "en",
            CancellationToken cancellationToken = default)
        {
            var risk = await _riskHandler.HandleAsync(
                scanId,
                language,
                cancellationToken);

            if (risk is null)
                return null;

            var decision = await _agent.AnalyzeAsync(
                risk,
                cancellationToken);

            return new AgentDecisionResponse(
                decision.Severity.ToString(),
                decision.Actions.Select(x => x.ToString()).ToList(),
                decision.Reason,
                decision.RequiresHumanReview);
        }
    }
}
