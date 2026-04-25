using EventTicketingAiPlatform.Contracts.ScanValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.UseCases.ScanValidation
{
    public sealed class ValidateTicketScanRequestValidator
    {
        public void Validate(ValidateTicketScanRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TicketCode))
                throw new ArgumentException("TicketCode is required.");

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                throw new ArgumentException("DeviceId is required.");

            if (string.IsNullOrWhiteSpace(request.GateId))
                throw new ArgumentException("GateId is required.");

            if (request.ScannedAtUtc == default)
                throw new ArgumentException("ScannedAtUtc is required.");
        }
    }
}
