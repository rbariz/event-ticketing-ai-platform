using EventTicketingAiPlatform.Application.UseCases.Risk;
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
        public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
        {
            var result = await _historyHandler.HandleAsync(cancellationToken);
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
    }


}
