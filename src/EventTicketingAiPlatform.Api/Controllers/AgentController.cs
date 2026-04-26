using EventTicketingAiPlatform.Application.UseCases.Agent;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/agent")]
    public sealed class AgentController : ControllerBase
    {
        private readonly AnalyzeScanWithAgentHandler _handler;

        public AgentController(AnalyzeScanWithAgentHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("analyze-scan/{scanId:guid}")]
        public async Task<IActionResult> AnalyzeScan(
            Guid scanId,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            var result = await _handler.HandleAsync(
                scanId,
                lang,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Scan not found",
                    Detail = $"No scan attempt found for id '{scanId}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(result);
        }
    }


}
