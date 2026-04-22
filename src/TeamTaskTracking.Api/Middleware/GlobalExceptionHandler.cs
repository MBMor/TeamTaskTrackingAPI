
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Application.Common.Exceptions;

namespace TeamTaskTracking.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(httpContext, validationException, cancellationToken);
                return true;

            case InvalidCredentialsException:
                await HandleProblemAsync(
                    httpContext,
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authentication failed.",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "Invalid credentials.",
                    type: "https://httpstatuses.com/401",
                    errorCode: "AUTH_INVALID_CREDENTIALS",
                    exception: exception,
                    cancellationToken: cancellationToken);
                return true;

            case InvalidRefreshTokenException:
                await HandleProblemAsync(
                    httpContext,
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authentication failed.",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "Invalid refresh token.",
                    type: "https://httpstatuses.com/401",
                    errorCode: "AUTH_INVALID_REFRESH_TOKEN",
                    exception: exception,
                    cancellationToken: cancellationToken);
                return true;

            case DuplicateEmailException:
                await HandleProblemAsync(
                    httpContext,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Email already in use.",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "The email address is already registered.",
                    type: "https://httpstatuses.com/409",
                    errorCode: "AUTH_DUPLICATE_EMAIL",
                    exception: exception,
                    cancellationToken: cancellationToken);
                return true;

            case InvalidOperationException:
                await HandleProblemAsync(
                    httpContext,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "The request could not be completed.",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "The request conflicts with the current state of the resource.",
                    type: "https://httpstatuses.com/409",
                    errorCode: "CONFLICT",
                    exception: exception,
                    cancellationToken: cancellationToken);
                return true;

            default:
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    httpContext.TraceIdentifier);

                await HandleProblemAsync(
                    httpContext,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred.",
                    detail: _environment.IsDevelopment()
                        ? exception.Message
                        : "The server was unable to process the request.",
                    type: "https://httpstatuses.com/500",
                    errorCode: "INTERNAL_SERVER_ERROR",
                    exception: exception,
                    cancellationToken: cancellationToken);
                return true;
        }
    }

    private async Task HandleValidationExceptionAsync(
        HttpContext httpContext,
        ValidationException validationException,
        CancellationToken cancellationToken)
    {
        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = "One or more validation errors occurred.",
            Type = "https://httpstatuses.com/400",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = "VALIDATION_ERROR";

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = validationException.GetType().FullName;
            problemDetails.Extensions["stackTrace"] = validationException.StackTrace;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private async Task HandleProblemAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string? detail,
        string type,
        string errorCode,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Type = type,
            Status = statusCode,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = errorCode;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().FullName;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        if (_environment.IsEnvironment("Testing"))
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
            problemDetails.Extensions["exceptionMessage"] = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
