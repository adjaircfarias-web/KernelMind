using Microsoft.AspNetCore.Mvc;
using KernelMind.Core.Services;
using System.Text;
using System.Text.Json;

namespace KernelMind.Api.Controllers;

/// <summary>
/// API controller for chat operations with streaming support
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
        _logger.LogInformation("Starting streaming chat for session: {SessionId}", sessionId);

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            await foreach (var chunk in _chatService.StreamMessageAsync(sessionId, request.Message, ct))
            {
                if (ct.IsCancellationRequested)
                    break;

                if (!string.IsNullOrEmpty(chunk))
                {
                    var sseData = JsonSerializer.Serialize(new StreamResponse(chunk, sessionId));
                    await Response.WriteAsync($"data: {sseData}\n\n", ct);
                    await Response.Body.FlushAsync(ct);
                }
            }
            
            await Response.WriteAsync($"data: [DONE]\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Streaming cancelled for session: {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during streaming for session: {SessionId}", sessionId);
            var errorData = JsonSerializer.Serialize(new StreamResponse($"Erro: {ex.Message}", sessionId));
            await Response.WriteAsync($"data: {errorData}\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }
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
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
        _logger.LogInformation("Starting raw streaming for session: {SessionId}", sessionId);

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no");

        try
        {
            await foreach (var chunk in _chatService.StreamMessageAsync(sessionId, request.Message, ct))
            {
                if (ct.IsCancellationRequested)
                    break;

                if (!string.IsNullOrEmpty(chunk))
                {
                    await Response.WriteAsync($"data: {chunk}\n\n", Encoding.UTF8, ct);
                    await Response.Body.FlushAsync(ct);
                }
            }
            
            await Response.WriteAsync("data: [DONE]\n\n", Encoding.UTF8, ct);
            await Response.Body.FlushAsync(ct);
        }
        catch (OperationCanceledException)
        {
            await Response.WriteAsync("data: [CANCELLED]\n\n", Encoding.UTF8, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during raw streaming for session: {SessionId}", sessionId);
            await Response.WriteAsync($"data: ERROR: {ex.Message}\n\n", Encoding.UTF8, ct);
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
public record ChatRequest(string Message, string? SessionId);

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
