using EventTicketingAiPlatform.Application.UseCases.Scans;
using EventTicketingAiPlatform.Application.UseCases.ScanValidation;
using EventTicketingAiPlatform.Contracts.ScanValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{

    [ApiController]
    [Route("api/scans")]
    public sealed class ScansController : ControllerBase
    {
        private readonly ValidateTicketScanHandler _validateHandler;
        private readonly GetScanHistoryHandler _historyHandler;

        public ScansController(
            ValidateTicketScanHandler validateHandler,
            GetScanHistoryHandler historyHandler)
        {
            _validateHandler = validateHandler;
            _historyHandler = historyHandler;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(
            ValidateTicketScanRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _validateHandler.HandleAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
        {
            var result = await _historyHandler.HandleAsync(cancellationToken);
            return Ok(result);
        }
    }
}
