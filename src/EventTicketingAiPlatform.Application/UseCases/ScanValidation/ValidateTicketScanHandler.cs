using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Domain.Entities;
using EventTicketingAiPlatform.Application.Domain.Enums;
using EventTicketingAiPlatform.Contracts.ScanValidation;
using System.Diagnostics;

namespace EventTicketingAiPlatform.Application.UseCases.ScanValidation
{
    public sealed class ValidateTicketScanHandler
    {
        private static readonly TimeSpan DuplicateScanWindow = TimeSpan.FromSeconds(2);

        private readonly ITicketRepository _ticketRepository;
        private readonly IScanAttemptRepository _scanAttemptRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidateTicketScanRequestValidator _validator;

        public ValidateTicketScanHandler(
            ITicketRepository ticketRepository,
            IScanAttemptRepository scanAttemptRepository,
            IUnitOfWork unitOfWork,
            ValidateTicketScanRequestValidator validator)
        {
            _ticketRepository = ticketRepository;
            _scanAttemptRepository = scanAttemptRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<ValidateTicketScanResponse> HandleAsync(
            ValidateTicketScanRequest request,
            CancellationToken cancellationToken = default)
        {
            _validator.Validate(request);

            var stopwatch = Stopwatch.StartNew();

            var ticketCode = request.TicketCode.Trim();
            var deviceId = request.DeviceId.Trim();
            var gateId = request.GateId.Trim();

            var ticket = await _ticketRepository.GetByCodeAsync(
                ticketCode,
                cancellationToken);

            if (ticket is null)
            {
                return await RejectAsync(
                    ticketCode,
                    null,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.TicketNotFound,
                    "Ticket not found.",
                    stopwatch,
                    cancellationToken);
            }

            var recentScan = await _scanAttemptRepository.GetRecentByTicketCodeAsync(
                ticketCode,
                request.ScannedAtUtc.Subtract(DuplicateScanWindow),
                cancellationToken);

            if (recentScan is not null && recentScan.Decision == ScanDecision.Accepted)
            {
                return await RejectAsync(
                    ticketCode,
                    ticket.Id,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.DuplicateScan,
                    "Duplicate scan detected.",
                    stopwatch,
                    cancellationToken);
            }

            if (ticket.Status == TicketStatus.Cancelled)
            {
                return await RejectAsync(
                    ticketCode,
                    ticket.Id,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.TicketCancelled,
                    "Ticket is cancelled.",
                    stopwatch,
                    cancellationToken);
            }

            if (ticket.IsExpired(request.ScannedAtUtc) || ticket.Status == TicketStatus.Expired)
            {
                return await RejectAsync(
                    ticketCode,
                    ticket.Id,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.TicketExpired,
                    "Ticket is expired.",
                    stopwatch,
                    cancellationToken);
            }

            if (!ticket.IsValidAt(request.ScannedAtUtc))
            {
                return await RejectAsync(
                    ticketCode,
                    ticket.Id,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.InvalidState,
                    "Ticket is not valid at this time.",
                    stopwatch,
                    cancellationToken);
            }

            if (ticket.IsAlreadyUsed())
            {
                return await RejectAsync(
                    ticketCode,
                    ticket.Id,
                    deviceId,
                    gateId,
                    request,
                    ScanReasonCode.TicketAlreadyUsed,
                    "Ticket has already been used.",
                    stopwatch,
                    cancellationToken);
            }

            ticket.MarkAsConsumed(request.ScannedAtUtc);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            var attempt = new ScanAttempt
            {
                Id = Guid.NewGuid(),
                TicketCode = ticketCode,
                TicketId = ticket.Id,
                DeviceId = deviceId,
                GateId = gateId,
                ScannedAtUtc = request.ScannedAtUtc,
                Decision = ScanDecision.Accepted,
                ReasonCode = ScanReasonCode.Ok,
                Source = request.Source,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };

            await _scanAttemptRepository.AddAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ValidateTicketScanResponse(
                Accepted: true,
                Decision: ScanDecision.Accepted.ToString(),
                ReasonCode: ScanReasonCode.Ok.ToString(),
                Message: "Ticket accepted.",
                TicketId: ticket.Id,
                ScanAttemptId: attempt.Id);
        }

        private async Task<ValidateTicketScanResponse> RejectAsync(
            string ticketCode,
            Guid? ticketId,
            string deviceId,
            string gateId,
            ValidateTicketScanRequest request,
            ScanReasonCode reasonCode,
            string message,
            Stopwatch stopwatch,
            CancellationToken cancellationToken)
        {
            var attempt = new ScanAttempt
            {
                Id = Guid.NewGuid(),
                TicketCode = ticketCode,
                TicketId = ticketId,
                DeviceId = deviceId,
                GateId = gateId,
                ScannedAtUtc = request.ScannedAtUtc,
                Decision = ScanDecision.Rejected,
                ReasonCode = reasonCode,
                Source = request.Source,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };

            await _scanAttemptRepository.AddAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ValidateTicketScanResponse(
                Accepted: false,
                Decision: ScanDecision.Rejected.ToString(),
                ReasonCode: reasonCode.ToString(),
                Message: message,
                TicketId: ticketId,
                ScanAttemptId: attempt.Id);
        }
    }
}
