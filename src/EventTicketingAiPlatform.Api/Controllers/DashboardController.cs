using EventTicketingAiPlatform.Application.UseCases.Dashboard;
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
            CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(cancellationToken);
            return Ok(result);
        }
    }


}
