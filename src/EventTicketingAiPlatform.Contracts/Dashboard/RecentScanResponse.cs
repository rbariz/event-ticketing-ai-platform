namespace EventTicketingAiPlatform.Contracts.Dashboard
{
    public sealed record RecentScanResponse(
    Guid Id,
    string TicketCode,
    string Decision,
    string ReasonCode,
    string GateId,
    string DeviceId,
    DateTime ScannedAtUtc,
    string? Source);
}
