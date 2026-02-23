using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace KernelMind.Api.Filters;

/// <summary>
/// Global validation action filter
/// </summary>
public class ValidationFilter : IActionFilter
{
    private readonly ILogger<ValidationFilter> _logger;

    public ValidationFilter(ILogger<ValidationFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new ValidationErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Timestamp = DateTime.UtcNow,
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
            _logger.LogWarning("Validation failed for {Path}: {Errors}", context.HttpContext.Request.Path, errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Can be used for post-action logic
    }
}

/// <summary>
/// Validation error response
/// </summary>
public class ValidationErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}

/// <summary>
/// Global exception filter for API controllers
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var response = new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = context.Exception.Message,
            Timestamp = DateTime.UtcNow,
            Path = context.HttpContext.Request.Path,
            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
        _logger.LogError(context.Exception, "Unhandled exception at {Path}", context.HttpContext.Request.Path);
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
/// Extension method to add validation filter
/// </summary>
public static class ValidationFilterExtensions
{
    public static void AddValidationFilters(this MvcOptions options)
    {
        options.Filters.Add<ValidationFilter>();
        options.Filters.Add<GlobalExceptionFilter>();
    }
}
