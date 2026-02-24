using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using KernelMind.Domain.Interfaces;
using KernelMind.Domain.Entities;
using Microsoft.Extensions.Logging;
using KernelMind.Core.Prompts;

namespace KernelMind.Core.Services;

/// <summary>
/// Service that orchestrates chat interactions using Semantic Kernel with Function Calling
/// </summary>
public class ChatService
{
        private static readonly TimeSpan LlmTimeout = TimeSpan.FromSeconds(60);

    private readonly Kernel _kernel;
    private readonly ILogger<ChatService> _logger;
    private readonly IChatSessionRepository _chatSessionRepository;

    public ChatService(
        Kernel kernel,
        ILogger<ChatService> logger,
        IChatSessionRepository chatSessionRepository)
    {
        _kernel = kernel;
        _logger = logger;
        _chatSessionRepository = chatSessionRepository;
    }

    /// <summary>
    /// Processes a user message and returns the assistant response with function calling
    /// </summary>
    public async Task<string> ProcessMessageAsync(
        string sessionId, 
        string message, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Processing message for session {SessionId}: {Message}", sessionId, message);

        try
        {
            var chatSession = await GetOrCreateSessionAsync(sessionId, ct);
            
            // Build chat history from database - limit to last 10 messages for performance
            var chatHistory = new ChatHistory(GetSystemPrompt());
            
            var recentMessages = chatSession.Messages
                .OrderBy(m => m.CreatedAt)
                .TakeLast(10)
                .ToList();
            
            foreach (var msg in recentMessages)
            {
                var role = msg.Role switch
                {
                    ChatRole.Assistant => AuthorRole.Assistant,
                    ChatRole.System => AuthorRole.System,
                    _ => AuthorRole.User
                };
                chatHistory.AddMessage(role, msg.Content);
            }

            // Add current user message
            chatHistory.AddUserMessage(message);

            // Get chat completion service with function calling enabled
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            
            // Enable function calling for llama3.1
            var executionSettings = new OllamaPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            using var llmCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            llmCts.CancelAfter(LlmTimeout);

            _logger.LogInformation("Starting LLM completion for session {SessionId}", sessionId);

            // Get response with potential function calls
            var response = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                _kernel,
                llmCts.Token);

            var responseText = response.Content ?? "Desculpe, não consegui processar sua mensagem.";

            // Save both messages to database
            await SaveMessageAsync(sessionId, "user", message, ct);
            await SaveMessageAsync(sessionId, "assistant", responseText, ct);

            _logger.LogInformation("Response generated for session {SessionId}", sessionId);

            return responseText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message for session {SessionId}", sessionId);
            return "Desculpe, ocorreu um erro ao processar sua mensagem. Tente novamente.";
        }
    }

    /// <summary>
    /// Streams the chat response - NOTE: Function calling is disabled in streaming mode
    /// because Ollama doesn't support it. Use ProcessMessageAsync for function calling.
    /// </summary>
    public async IAsyncEnumerable<string> StreamMessageAsync(
        string sessionId,
        string message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        _logger.LogInformation("Streaming message for session {SessionId}: {Message}", sessionId, message);

        var chatHistory = new ChatHistory(GetSystemPromptForStreaming());
        
        // Load recent conversation history (limit to last 10 messages for consistency and performance)
        var chatSession = await GetOrCreateSessionAsync(sessionId, ct);
        var recentMessages = chatSession.Messages
            .OrderBy(m => m.CreatedAt)
            .TakeLast(10)
            .ToList();

        foreach (var msg in recentMessages)
        {
            var role = msg.Role switch
            {
                ChatRole.Assistant => AuthorRole.Assistant,
                ChatRole.System => AuthorRole.System,
                _ => AuthorRole.User
            };
            chatHistory.AddMessage(role, msg.Content);
        }

        chatHistory.AddUserMessage(message);

        // Save user message
        await SaveMessageAsync(sessionId, "user", message, ct);

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        
        // NOTE: Function calling is DISABLED in streaming mode because Ollama doesn't support it
        // Use the synchronous ProcessMessageAsync endpoint for function calling
        var executionSettings = new OllamaPromptExecutionSettings
        {
            // FunctionChoiceBehavior is NOT set for streaming - Ollama limitation
        };

        using var llmCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        llmCts.CancelAfter(LlmTimeout);

        var responseBuilder = new System.Text.StringBuilder();

        await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory,
            executionSettings,
            _kernel,
            llmCts.Token))
        {
            if (!string.IsNullOrEmpty(chunk.Content))
            {
                responseBuilder.Append(chunk.Content);
                yield return chunk.Content;
            }
        }

        // Save assistant response
        var fullResponse = responseBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(fullResponse))
        {
            await SaveMessageAsync(sessionId, "assistant", fullResponse, ct);
        }
    }

    private async Task<ChatSession> GetOrCreateSessionAsync(string sessionId, CancellationToken ct)
    {
        var session = await _chatSessionRepository.GetByTokenAsync(sessionId, ct);
        
        if (session == null)
        {
            session = new ChatSession
            {
                SessionToken = sessionId,
                IsActive = true
            };
            await _chatSessionRepository.CreateAsync(session, ct);
        }

        return session;
    }

    private async Task SaveMessageAsync(string sessionId, string role, string content, CancellationToken ct)
    {
        var session = await _chatSessionRepository.GetByTokenAsync(sessionId, ct);
        if (session != null)
        {
            var message = new ChatMessage
            {
                SessionId = session.Id,
                Role = role == "assistant" ? ChatRole.Assistant : ChatRole.User,
                Content = content
            };
            await _chatSessionRepository.AddMessageAsync(message, ct);
        }
    }

    private string GetSystemPrompt() => ChatPrompts.SystemPrompt;

    private string GetSystemPromptForStreaming() => ChatPrompts.StreamingSystemPrompt;
}
