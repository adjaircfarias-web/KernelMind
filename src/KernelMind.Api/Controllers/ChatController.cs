using Microsoft.AspNetCore.Mvc;
using KernelMind.Core.Services;
using System.Runtime.CompilerServices;

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
    /// Streams chat responses using HTTP Streaming (IAsyncEnumerable)
    /// </summary>
    [HttpPost("stream")]
    public async IAsyncEnumerable<string> StreamMessage(
        [FromBody] ChatRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        _logger.LogInformation("Streaming chat message: {Message}", request.Message);
        
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
        
        await foreach (var chunk in _chatService.StreamMessageAsync(sessionId, request.Message, ct))
        {
            yield return chunk;
        }
    }
}

public record ChatRequest(string Message, string? SessionId);
public record ChatResponse(string Content, string SessionId, DateTime Timestamp);
