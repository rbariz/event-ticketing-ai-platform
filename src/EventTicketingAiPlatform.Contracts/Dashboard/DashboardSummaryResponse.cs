using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Dashboard
{
    public sealed record DashboardSummaryResponse(
    int TotalScans,
    int AcceptedScans,
    int RejectedScans,
    int DuplicateScans,
    int ExpiredScans,
    int AlreadyUsedScans,
    int HighRiskScans,
    IReadOnlyList<DashboardReasonCountResponse> TopRejectReasons,
    IReadOnlyList<DashboardGateCountResponse> TopGates,
    IReadOnlyList<RecentScanResponse> RecentScans);
}
