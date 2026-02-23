using System.Diagnostics;
using System.Text.Json;

namespace KernelMind.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized access"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
            OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Request timeout"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        response.StatusCode = statusCode;

        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path,
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier
        };

        _logger.LogError(exception, "Request failed: {Path}", context.Request.Path);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}

/// <summary>
/// Standardized error response
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Path { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
}

/// <summary>
/// Extension method to add exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
