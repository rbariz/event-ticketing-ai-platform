using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;

namespace EventTicketingAiPlatform.Application.Risk
{
    public sealed class RuleBasedRiskScoringService : IRiskScoringService
    {
        public AntifraudRiskAssessment Assess(
            Ticket? ticket,
            ScanAttempt attempt,
            ScanAttempt? recentScan)
        {
            var score = 0;
            var signals = new List<string>();

            if (ticket is null)
            {
                score += 90;
                signals.Add("unknown_ticket");
            }

            if (attempt.ReasonCode == ScanReasonCode.DuplicateScan)
            {
                score += 70;
                signals.Add("duplicate_scan");
            }

            if (attempt.ReasonCode == ScanReasonCode.TicketAlreadyUsed)
            {
                score += 80;
                signals.Add("already_used");
            }

            if (attempt.ReasonCode == ScanReasonCode.TicketExpired)
            {
                score += 60;
                signals.Add("expired_ticket");
            }

            if (attempt.ReasonCode == ScanReasonCode.TicketCancelled)
            {
                score += 75;
                signals.Add("cancelled_ticket");
            }

            if (recentScan is not null &&
                recentScan.DeviceId != attempt.DeviceId)
            {
                score += 40;
                signals.Add("multi_device");
            }

            score = Math.Min(score, 100);

            var level = score switch
            {
                >= 80 => "High",
                >= 50 => "Medium",
                >= 20 => "Low",
                _ => "Low"
            };

            var action = level switch
            {
                "High" => "ManualReview",
                "Medium" => "Monitor",
                _ => "Allow"
            };

            return new AntifraudRiskAssessment
            {
                RiskScore = score,
                RiskLevel = level,
                RecommendedAction = action,
                RiskSignals = signals
            };
        }
    }
}
