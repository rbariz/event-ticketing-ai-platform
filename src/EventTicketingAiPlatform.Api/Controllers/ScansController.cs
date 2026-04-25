using EventTicketingAiPlatform.Application.UseCases.Risk;
using EventTicketingAiPlatform.Application.UseCases.Scans;
using EventTicketingAiPlatform.Application.UseCases.ScanValidation;
using EventTicketingAiPlatform.Contracts.Query;
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
        private readonly GetScanRiskAssessmentHandler _riskHandler;

        public ScansController(
            ValidateTicketScanHandler validateHandler,
            GetScanHistoryHandler historyHandler,
            GetScanRiskAssessmentHandler riskHandler)
        {
            _validateHandler = validateHandler;
            _historyHandler = historyHandler;
            _riskHandler = riskHandler;
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
        public async Task<IActionResult> GetHistory(
    [FromQuery] DateTime? fromUtc,
    [FromQuery] DateTime? toUtc,
    [FromQuery] string? gateId,
    [FromQuery] string? source,
    [FromQuery] string? decision,
    [FromQuery] string? reasonCode,
    CancellationToken cancellationToken)
        {
            var query = new ScanQueryRequest(
                fromUtc,
                toUtc,
                gateId,
                source,
                decision,
                reasonCode);

            var result = await _historyHandler.HandleAsync(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}/risk")]
        public async Task<IActionResult> GetRisk(
        Guid id,
        [FromQuery] string lang = "en",
        CancellationToken cancellationToken = default)
        {
            var result = await _riskHandler.HandleAsync(id, lang, cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Scan not found",
                    Detail = $"No scan attempt found for id '{id}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent(
    [FromQuery] int count = 20,
    CancellationToken cancellationToken = default)
        {
            if (count <= 0)
                count = 20;

            if (count > 200)
                count = 200;

            var scans = await _historyHandler.HandleAsync(
    new ScanQueryRequest(null, null, null, null, null, null),
    cancellationToken);

            var result = scans
                .OrderByDescending(x => x.ScannedAtUtc)
                .Take(count)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var scans = await _historyHandler.HandleAsync(
    new ScanQueryRequest(null, null, null, null, null, null),
    cancellationToken);
            var scan = scans.FirstOrDefault(x => x.Id == id);

            if (scan is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Scan not found",
                    Detail = $"No scan attempt found for id '{id}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(scan);
        }
    }


}
