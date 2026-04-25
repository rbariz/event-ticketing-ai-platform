using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Contracts.Dashboard;


namespace EventTicketingAiPlatform.Application.UseCases.Dashboard
{
    
    public sealed class GetDashboardSummaryHandler
    {
        private readonly IScanAttemptRepository _scanRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IRiskScoringService _riskScoringService;

        public GetDashboardSummaryHandler(
            IScanAttemptRepository scanRepository,
            ITicketRepository ticketRepository,
            IRiskScoringService riskScoringService)
        {
            _scanRepository = scanRepository;
            _ticketRepository = ticketRepository;
            _riskScoringService = riskScoringService;
        }

        public async Task<DashboardSummaryResponse> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var scans = await _scanRepository.GetAllAsync(cancellationToken);

            var total = scans.Count;
            var accepted = scans.Count(x => x.Decision == ScanDecision.Accepted);
            var rejected = scans.Count(x => x.Decision == ScanDecision.Rejected);

            var duplicate = scans.Count(x => x.ReasonCode == ScanReasonCode.DuplicateScan);
            var expired = scans.Count(x => x.ReasonCode == ScanReasonCode.TicketExpired);
            var alreadyUsed = scans.Count(x => x.ReasonCode == ScanReasonCode.TicketAlreadyUsed);

            var highRisk = 0;

            foreach (var scan in scans)
            {
                var ticket = await _ticketRepository.GetByCodeAsync(
                    scan.TicketCode,
                    cancellationToken);

                var recentScan = await _scanRepository.GetRecentByTicketCodeAsync(
                    scan.TicketCode,
                    scan.ScannedAtUtc.AddMinutes(-5),
                    cancellationToken);

                var risk = _riskScoringService.Assess(ticket, scan, recentScan);

                if (risk.RiskLevel is "High" or "Critical")
                    highRisk++;
            }

            var topRejectReasons = scans
                .Where(x => x.Decision == ScanDecision.Rejected)
                .GroupBy(x => x.ReasonCode.ToString())
                .OrderByDescending(x => x.Count())
                .Take(5)
                .Select(x => new DashboardReasonCountResponse(x.Key, x.Count()))
                .ToList();

            var topGates = scans
                .GroupBy(x => x.GateId)
                .OrderByDescending(x => x.Count())
                .Take(5)
                .Select(x => new DashboardGateCountResponse(x.Key, x.Count()))
                .ToList();

            var recent = scans
                .OrderByDescending(x => x.ScannedAtUtc)
                .Take(10)
                .Select(x => new RecentScanResponse(
                    x.Id,
                    x.TicketCode,
                    x.Decision.ToString(),
                    x.ReasonCode.ToString(),
                    x.GateId,
                    x.DeviceId,
                    x.ScannedAtUtc,
                    x.Source))
                .ToList();

            return new DashboardSummaryResponse(
                total,
                accepted,
                rejected,
                duplicate,
                expired,
                alreadyUsed,
                highRisk,
                topRejectReasons,
                topGates,
                recent);
        }
    }
}
