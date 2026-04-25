using EventTicketingAiPlatform.Application.UseCases.Dashboard;
using EventTicketingAiPlatform.Contracts.Query;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public sealed class DashboardController : ControllerBase
    {
        private readonly GetDashboardSummaryHandler _handler;

        public DashboardController(GetDashboardSummaryHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
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

            var result = await _handler.HandleAsync(query, cancellationToken);
            return Ok(result);
        }
    }


}
