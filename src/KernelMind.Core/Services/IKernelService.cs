using Microsoft.Extensions.AI;

namespace KernelMind.Core.Services;

/// <summary>
/// Interface for KernelMind AI services
/// </summary>
public interface IKernelService
{
    /// <summary>
    /// Gets the chat client for LLM interactions
    /// </summary>
    IChatClient ChatClient { get; }

    /// <summary>
    /// Processes a user message and returns the assistant response
    /// </summary>
    Task<string> ProcessMessageAsync(string sessionId, string message, CancellationToken ct = default);

    /// <summary>
    /// Streams the chat response using IAsyncEnumerable
    /// </summary>
    IAsyncEnumerable<string> StreamMessageAsync(string sessionId, string message, CancellationToken ct = default);
}
