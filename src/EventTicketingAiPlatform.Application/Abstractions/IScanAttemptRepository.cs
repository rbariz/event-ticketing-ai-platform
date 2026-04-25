using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface IScanAttemptRepository
    {
        Task AddAsync(
            ScanAttempt scanAttempt,
            CancellationToken cancellationToken = default);

        Task<ScanAttempt?> GetRecentByTicketCodeAsync(
            string ticketCode,
            DateTime sinceUtc,
            CancellationToken cancellationToken = default);
    }
}
