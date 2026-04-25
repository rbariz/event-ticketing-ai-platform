using EventTicketingAiPlatform.Application.Exceptions;
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
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.TicketCode))
                errors["ticketCode"] = ["TicketCode is required."];

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                errors["deviceId"] = ["DeviceId is required."];

            if (string.IsNullOrWhiteSpace(request.GateId))
                errors["gateId"] = ["GateId is required."];

            if (request.ScannedAtUtc == default)
                errors["scannedAtUtc"] = ["ScannedAtUtc is required."];

            if (errors.Count > 0)
                throw new RequestValidationException(errors);
        }
    }
}
