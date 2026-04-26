using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Agent;
using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.UseCases.Risk;
using EventTicketingAiPlatform.Contracts.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.Agent
{

    public sealed class AnalyzeScanWithAgentHandler
    {
        private readonly GetScanRiskAssessmentHandler _riskHandler;
        private readonly IAntifraudAgent _agent;
        private readonly IAgentDecisionLogRepository _logRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnalyzeScanWithAgentHandler(
            GetScanRiskAssessmentHandler riskHandler,
            IAntifraudAgent agent,
            IAgentDecisionLogRepository logRepository,
            IUnitOfWork unitOfWork)
        {
            _riskHandler = riskHandler;
            _agent = agent;
            _logRepository = logRepository;
            _unitOfWork = unitOfWork;
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

            var log = new AgentDecisionLog
            {
                Id = Guid.NewGuid(),
                ScanAttemptId = scanId,
                RiskScore = risk.RiskScore,
                RiskLevel = risk.RiskLevel,
                Severity = decision.Severity.ToString(),
                Actions = string.Join(",", decision.Actions.Select(x => x.ToString())),
                Reason = decision.Reason,
                RequiresHumanReview = decision.RequiresHumanReview,
                Provider = "RuleBasedAgent",
                CreatedAtUtc = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AgentDecisionResponse(
                decision.Severity.ToString(),
                decision.Actions.Select(x => x.ToString()).ToList(),
                decision.Reason,
                decision.RequiresHumanReview);
        }
    }
}
