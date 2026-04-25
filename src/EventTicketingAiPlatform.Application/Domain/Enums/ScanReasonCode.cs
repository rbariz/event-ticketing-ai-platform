namespace EventTicketingAiPlatform.Application.Domain.Enums
{
    public enum ScanReasonCode
    {
        Ok = 1,
        TicketNotFound = 2,
        TicketExpired = 3,
        TicketAlreadyUsed = 4,
        DuplicateScan = 5,
        TicketCancelled = 6,
        InvalidState = 7
    }
}
