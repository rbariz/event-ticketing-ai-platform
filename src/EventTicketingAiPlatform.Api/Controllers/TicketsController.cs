using EventTicketingAiPlatform.Application.UseCases.Tickets;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public sealed class TicketsController : ControllerBase
    {
        private readonly GetTicketByCodeHandler _handler;

        public TicketsController(GetTicketByCodeHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(
            string code,
            CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(code, cancellationToken);

            if (result is null)
                return NotFound(new ProblemDetails
                {
                    Title = "Ticket not found",
                    Detail = $"No ticket found for code '{code}'.",
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                });

            return Ok(result);
        }
    }
}
