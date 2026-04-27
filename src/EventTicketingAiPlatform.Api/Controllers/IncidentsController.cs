using EventTicketingAiPlatform.Application.UseCases.Incidents;
using EventTicketingAiPlatform.Contracts.Incidents;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/incidents")]
    public sealed class IncidentsController : ControllerBase
    {
        private readonly GetIncidentsHandler _getIncidentsHandler;
        private readonly GetIncidentByIdHandler _getIncidentByIdHandler;
        private readonly AssignIncidentHandler _assignIncidentHandler;
        private readonly ResolveIncidentHandler _resolveIncidentHandler;

        public IncidentsController(
            GetIncidentsHandler getIncidentsHandler,
            GetIncidentByIdHandler getIncidentByIdHandler,
            AssignIncidentHandler assignIncidentHandler,
            ResolveIncidentHandler resolveIncidentHandler)
        {
            _getIncidentsHandler = getIncidentsHandler;
            _getIncidentByIdHandler = getIncidentByIdHandler;
            _assignIncidentHandler = assignIncidentHandler;
            _resolveIncidentHandler = resolveIncidentHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidents(
            [FromQuery] string? status = null,
            [FromQuery] string? severity = null,
            [FromQuery] int count = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _getIncidentsHandler.HandleAsync(
                status,
                severity,
                count,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _getIncidentByIdHandler.HandleAsync(
                id,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Incident not found",
                    Detail = $"No incident found for id '{id}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(result);
        }

        [HttpPost("{id:guid}/assign")]
        public async Task<IActionResult> Assign(
            Guid id,
            AssignIncidentRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _assignIncidentHandler.HandleAsync(
                id,
                request,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Incident not found",
                    Detail = $"No incident found for id '{id}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(result);
        }

        [HttpPost("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve(
            Guid id,
            ResolveIncidentRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _resolveIncidentHandler.HandleAsync(
                id,
                request,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Incident not found",
                    Detail = $"No incident found for id '{id}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });
            }

            return Ok(result);
        }
    }


}
