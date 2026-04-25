namespace EventTicketingAiPlatform.Contracts.ScanValidation
{
    public sealed record ValidateTicketScanResponse(
    bool Accepted,
    string Decision,
    string ReasonCode,
    string Message,
    Guid? TicketId,
    Guid ScanAttemptId);
}
