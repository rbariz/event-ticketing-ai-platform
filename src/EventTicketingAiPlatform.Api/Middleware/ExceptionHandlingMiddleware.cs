using System.Net;
using System.Text.Json;
using EventTicketingAiPlatform.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingAiPlatform.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception ex,
        ILogger logger)
    {
        switch (ex)
        {
            case RequestValidationException validationEx:
                {
                    var problem = new ValidationProblemDetails(
                        validationEx.Errors.ToDictionary(x => x.Key, x => x.Value))
                    {
                        Title = "Validation failed",
                        Detail = "One or more validation errors occurred.",
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400"
                    };

                    problem.Extensions["traceId"] = context.TraceIdentifier;

                    await WriteProblemAsync(context, problem);
                    return;
                }

            case ArgumentException:
                {
                    var problem = new ProblemDetails
                    {
                        Title = "Bad request",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400"
                    };

                    problem.Extensions["traceId"] = context.TraceIdentifier;

                    await WriteProblemAsync(context, problem);
                    return;
                }

            default:
                {
                    logger.LogError(ex, "Unhandled exception occurred.");

                    var problem = new ProblemDetails
                    {
                        Title = "Internal server error",
                        Detail = "An unexpected error occurred.",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Type = "https://httpstatuses.com/500"
                    };

                    problem.Extensions["traceId"] = context.TraceIdentifier;

                    await WriteProblemAsync(context, problem);
                    return;
                }
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        ProblemDetails problem)
    {
        context.Response.StatusCode =
            problem.Status ?? StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(
            problem,
            problem.GetType(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json, context.RequestAborted);
    }
}