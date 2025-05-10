using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bookify.Api.Extensions;

public class GlobalErrorHandler(
    ILogger<GlobalErrorHandler> logger,
    IOptions<ErrorHandlingOptions> options,
    ActivitySource activitySource)
    : IExceptionHandler
{
    private readonly ErrorHandlingOptions _options = options.Value;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Ghi log lỗi với Correlation ID (nếu có)
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? context.TraceIdentifier;
        using var activity = activitySource.StartActivity("ErrorHandling");
        activity?.SetTag("correlation.id", correlationId);
        activity?.SetTag("exception.type", exception.GetType().Name);

        logger.LogError(
            exception,
            "Error occurred. CorrelationId: {CorrelationId}, Message: {Message}",
            correlationId,
            exception.Message);

        // Xây dựng Problem Details
        var problemDetails = CreateProblemDetails(context, exception);

        // Thiết lập phản hồi
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        // Ghi phản hồi JSON
        await context.Response.WriteAsJsonAsync(
            problemDetails,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase },
            cancellationToken);

        return true; // Xác nhận rằng lỗi đã được xử lý
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var (status, type, title, detail, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                "Validation Error",
                "One or more validation errors occurred.",
                validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "https://tools.ietf.org/html/rfc7235#section-3.1",
                "Unauthorized",
                "You are not authorized to access this resource.",
                null),

            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Not Found",
                notFound.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                "Server Error",
                _options.ExposeExceptionDetails
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later.",
                null)
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Type = type,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Thêm Correlation ID vào extensions
        problemDetails.Extensions["correlationId"] = context.TraceIdentifier;

        // Thêm lỗi chi tiết nếu có
        if (errors != null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        return problemDetails;
    }
}

// Cấu hình tùy chọn
public class ErrorHandlingOptions
{
    public bool ExposeExceptionDetails { get; set; } = false;
}

// Định nghĩa NotFoundException tùy chỉnh
public abstract class NotFoundException(string message) : Exception(message);