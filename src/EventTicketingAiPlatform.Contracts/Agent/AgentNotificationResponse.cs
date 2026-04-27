namespace EventTicketingAiPlatform.Contracts.Agent
{
    public sealed record AgentNotificationResponse(
    Guid Id,
    Guid ScanAttemptId,
    string Severity,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAtUtc,
    DateTime? ReadAtUtc);
}
