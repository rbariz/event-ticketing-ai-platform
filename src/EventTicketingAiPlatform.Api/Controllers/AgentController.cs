using EventTicketingAiPlatform.Application.UseCases.Agent;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/agent")]
    public sealed class AgentController : ControllerBase
    {
        private readonly AnalyzeScanWithAgentHandler _handler;
        private readonly GetRecentAgentDecisionLogsHandler _logsHandler;
        private readonly GetAgentNotificationsHandler _notificationsHandler;
        private readonly MarkAgentNotificationAsReadHandler _markNotificationAsReadHandler;

        public AgentController(AnalyzeScanWithAgentHandler handler, GetRecentAgentDecisionLogsHandler logsHandler, GetAgentNotificationsHandler notificationsHandler, MarkAgentNotificationAsReadHandler markNotificationAsReadHandler)
        {
            _handler = handler;
            _logsHandler = logsHandler;
            _notificationsHandler = notificationsHandler;
            _markNotificationAsReadHandler = markNotificationAsReadHandler;
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

        [HttpGet("decision-logs")]
        public async Task<IActionResult> GetDecisionLogs(
    [FromQuery] int count = 20,
    CancellationToken cancellationToken = default)
        {
            var result = await _logsHandler.HandleAsync(
                count,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
    [FromQuery] bool unreadOnly = false,
    [FromQuery] int count = 20,
    CancellationToken cancellationToken = default)
        {
            var result = await _notificationsHandler.HandleAsync(
                unreadOnly,
                count,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("notifications/{id:guid}/mark-read")]
        public async Task<IActionResult> MarkNotificationAsRead(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await _markNotificationAsReadHandler.HandleAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }


}
