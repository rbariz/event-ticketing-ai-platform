using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Contracts.Scans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.Scans
{
    public sealed class GetScanHistoryHandler
    {
        private readonly IScanAttemptRepository _repository;

        public GetScanHistoryHandler(IScanAttemptRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ScanAttemptResponse>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);

            return items
                .Select(x => new ScanAttemptResponse(
                    x.Id,
                    x.TicketCode,
                    x.TicketId,
                    x.DeviceId,
                    x.GateId,
                    x.ScannedAtUtc,
                    x.Decision.ToString(),
                    x.ReasonCode.ToString(),
                    x.Source,
                    x.ProcessingTimeMs))
                .ToList();
        }
    }
}
