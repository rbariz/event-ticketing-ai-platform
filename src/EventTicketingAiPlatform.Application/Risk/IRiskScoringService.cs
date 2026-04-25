using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Application.Risk
{
    public interface IRiskScoringService
    {
        AntifraudRiskAssessment Assess(
            Ticket? ticket,
            ScanAttempt attempt,
            ScanAttempt? recentScan);
    }
}
