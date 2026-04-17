using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TeamTaskTracking.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if(exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(x => x.ErrorMessage).ToArray());

            var validationProblemDetails = new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors occurred.",
                Type = "https://httpstatuses.com/400",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            };

            validationProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            if (_environment.IsDevelopment())
            {
                validationProblemDetails.Extensions["exception"] = validationException.GetType().Name;
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);

            return true;
        }

        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Detail = _environment.IsDevelopment()
                ? exception.Message
                : "The server was unable to process the request.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().FullName;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
