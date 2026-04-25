using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Contracts.Risk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.Risk
{
    public sealed class GetScanRiskAssessmentHandler
    {
        private readonly IScanAttemptRepository _scanRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IRiskScoringService _riskScoringService;
        private readonly IRiskExplanationService _riskExplanationService;

        public GetScanRiskAssessmentHandler(
            IScanAttemptRepository scanRepository,
            ITicketRepository ticketRepository,
            IRiskScoringService riskScoringService,
            IRiskExplanationService riskExplanationService)
        {
            _scanRepository = scanRepository;
            _ticketRepository = ticketRepository;
            _riskScoringService = riskScoringService;
            _riskExplanationService = riskExplanationService;
        }

        public async Task<RiskAssessmentResponse?> HandleAsync(
            Guid scanId,
            string language = "en",
            CancellationToken cancellationToken = default)
        {
            var scan = await _scanRepository.GetByIdAsync(scanId, cancellationToken);

            if (scan is null)
                return null;

            var ticket = await _ticketRepository.GetByCodeAsync(
                scan.TicketCode,
                cancellationToken);

            var recentScan = await _scanRepository.GetRecentByTicketCodeAsync(
                scan.TicketCode,
                scan.ScannedAtUtc.AddMinutes(-5),
                cancellationToken);

            var risk = _riskScoringService.Assess(ticket, scan, recentScan);

            var explanation = await _riskExplanationService.GenerateExplanationAsync(
                risk,
                language,
                cancellationToken);

            return new RiskAssessmentResponse(
                risk.RiskScore,
                risk.RiskLevel,
                explanation.OperatorMessage,
                risk.RiskSignals,
                risk.RecommendedAction,
                explanation.Summary,
                explanation.Confidence,
                explanation.Provider);
        }
    }
}
