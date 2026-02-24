using Microsoft.AspNetCore.Mvc;
using KernelMind.Core.Services;
using KernelMind.Api.Filters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace KernelMind.Api.Controllers;

/// <summary>
/// API controller for chat operations with streaming support
/// </summary>
[Route("api/[controller]")]
[TypeFilter(typeof(ValidationFilter), Order = 2)]
[TypeFilter(typeof(GlobalExceptionFilter), Order = 1)]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        ChatService chatService,
        ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a message to the chatbot and gets a response
    /// </summary>
    [HttpPost("message")]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        [FromBody] ChatRequest request,
        CancellationToken ct)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Message is required" });
        }
        
        _logger.LogInformation("Received chat message: {Message}", request.Message);
        
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
        var response = await _chatService.ProcessMessageAsync(sessionId, request.Message, ct);

        return Ok(new ChatResponse(response, sessionId, DateTime.UtcNow));
    }

    /// <summary>
    /// Streams chat responses using HTTP Streaming (Server-Sent Events)
    /// </summary>
    [HttpPost("stream")]
    public async Task StreamMessage(
        [FromBody] ChatRequest request,
        CancellationToken ct)
    {
        await StreamInternalAsync(request, raw: false, ct);
    }

    /// <summary>
    /// Alternative streaming endpoint using IAsyncEnumerable with proper SSE formatting
    /// Returns raw SSE format for better compatibility
    /// </summary>
    [HttpPost("stream/raw")]
    public async Task StreamMessageRaw(
        [FromBody] ChatRequest request,
        CancellationToken ct)
    {
        await StreamInternalAsync(request, raw: true, ct);
    }

    private async Task StreamInternalAsync(
        ChatRequest? request,
        bool raw,
        CancellationToken ct)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("{\"error\": \"Message is required\"}", ct);
            return;
        }

        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Starting {Mode} streaming for session: {SessionId}",
            raw ? "raw" : "standard",
            sessionId);

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        if (raw)
        {
            Response.Headers.Append("X-Accel-Buffering", "no");
        }

        try
        {
            await foreach (var chunk in _chatService.StreamMessageAsync(sessionId, request.Message, ct))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                if (string.IsNullOrEmpty(chunk))
                {
                    continue;
                }

                if (raw)
                {
                    await Response.WriteAsync($"data: {chunk}\n\n", Encoding.UTF8, ct);
                }
                else
                {
                    var sseData = JsonSerializer.Serialize(new StreamResponse(chunk, sessionId));
                    await Response.WriteAsync($"data: {sseData}\n\n", ct);
                }

                await Response.Body.FlushAsync(ct);
            }

            var doneEvent = raw ? "data: [DONE]\n\n" : "data: [DONE]\n\n";
            await Response.WriteAsync(doneEvent, raw ? Encoding.UTF8 : null, ct);
            await Response.Body.FlushAsync(ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Streaming cancelled for session: {SessionId}", sessionId);

            if (raw)
            {
                await Response.WriteAsync("data: [CANCELLED]\n\n", Encoding.UTF8, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during streaming for session: {SessionId}", sessionId);

            if (raw)
            {
                await Response.WriteAsync($"data: ERROR: {ex.Message}\n\n", Encoding.UTF8, ct);
            }
            else
            {
                var errorData = JsonSerializer.Serialize(new StreamResponse($"Erro: {ex.Message}", sessionId));
                await Response.WriteAsync($"data: {errorData}\n\n", ct);
            }
        }
    }

    /// <summary>
    /// Health check endpoint for the chat service
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new ChatHealthResponse(
            Status: "healthy",
            Service: "KernelMind.Chat",
            Timestamp: DateTime.UtcNow,
            Version: "1.0.0"
        ));
    }
}

/// <summary>
/// Request DTOs
/// </summary>
public class ChatRequest
{
    [Required]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }
}

/// <summary>
/// Response DTOs
/// </summary>
public record ChatResponse(string Content, string SessionId, DateTime Timestamp);

public record StreamResponse(string Chunk, string SessionId);

public record ChatHealthResponse(
    string Status,
    string Service,
    DateTime Timestamp,
    string Version
);
