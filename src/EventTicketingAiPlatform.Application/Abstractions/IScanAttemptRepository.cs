using EventTicketingAiPlatform.Application.Domain.Entities;

namespace EventTicketingAiPlatform.Application.Abstractions
{
    public interface IScanAttemptRepository
    {
        Task AddAsync(
            ScanAttempt scanAttempt,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ScanAttempt>> GetAllAsync(
    CancellationToken cancellationToken = default);

        Task<ScanAttempt?> GetRecentByTicketCodeAsync(
            string ticketCode,
            DateTime sinceUtc,
            CancellationToken cancellationToken = default);

        Task<ScanAttempt?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ScanAttempt>> GetRecentAsync(
    int count,
    CancellationToken cancellationToken = default);


        Task<IReadOnlyList<ScanAttempt>> SearchAsync(
    DateTime? fromUtc,
    DateTime? toUtc,
    string? gateId,
    string? source,
    string? decision,
    string? reasonCode,
    CancellationToken cancellationToken = default);
    }
}
