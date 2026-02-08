using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for maintaining conversation context
/// </summary>
public class ContextPlugin
{
    private readonly ILogger<ContextPlugin> _logger;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly Dictionary<string, Dictionary<string, object>> _contexts = new();

    public ContextPlugin(
        ILogger<ContextPlugin> logger,
        IChatSessionRepository chatSessionRepository)
    {
        _logger = logger;
        _chatSessionRepository = chatSessionRepository;
    }

    /// <summary>
    /// Saves a message to the conversation history
    /// </summary>
    public async Task<string> SaveMessageAsync(
        string sessionToken,
        string role,
        string content,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Saving message for session {Session}", sessionToken);
        
        try
        {
            var chatRole = Enum.Parse<ChatRole>(role, ignoreCase: true);
            
            var session = await _chatSessionRepository.GetByTokenAsync(sessionToken, ct);
            
            if (session == null)
            {
                session = new ChatSession
                {
                    SessionToken = sessionToken,
                    IsActive = true
                };
                await _chatSessionRepository.CreateAsync(session, ct);
            }
            
            var message = new ChatMessage
            {
                SessionId = session.Id,
                Role = chatRole,
                Content = content
            };
            
            await _chatSessionRepository.AddMessageAsync(message, ct);
            
            return $"✅ Mensagem salva no histórico.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message");
            return $"ℹ️ Mensagem processada (erro ao salvar no histórico).";
        }
    }

    /// <summary>
    /// Gets the conversation history for a session
    /// </summary>
    public async Task<string> GetHistoryAsync(
        string sessionToken,
        int limit = 10,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting history for session {Session}", sessionToken);
        
        var session = await _chatSessionRepository.GetByTokenAsync(sessionToken, ct);
        
        if (session == null || !session.Messages.Any())
        {
            return $"📭 **Histórico de Conversa**\n\n" +
                   $"Nenhuma mensagem encontrada nesta sessão.\n\n" +
                   $"💡 Comece a conversar para ver o histórico aqui!";
        }
        
        var messages = session.Messages
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .Reverse()
            .Select(m => $"**{m.Role}:** {m.Content}");
        
        return $"📋 **Histórico de Conversa** (últimas {limit} mensagens)\n\n" +
               string.Join("\n\n", messages);
    }

    /// <summary>
    /// Stores information in the conversation context
    /// </summary>
    public string SetContext(string sessionToken, string key, string value)
    {
        _logger.LogInformation("Setting context for session {Session}: {Key} = {Value}", 
            sessionToken, key, value);
        
        if (!_contexts.ContainsKey(sessionToken))
            _contexts[sessionToken] = new Dictionary<string, object>();
        
        _contexts[sessionToken][key] = value;
        
        return $"✅ Informação '{key}' armazenada com sucesso.";
    }

    /// <summary>
    /// Retrieves information from the conversation context
    /// </summary>
    public string GetContext(string sessionToken, string key)
    {
        _logger.LogInformation("Getting context for session {Session}: {Key}", sessionToken, key);
        
        if (!_contexts.TryGetValue(sessionToken, out var context))
            return $"ℹ️ Nenhuma informação encontrada para a chave '{key}'.";
        
        if (!context.TryGetValue(key, out var value))
            return $"ℹ️ Nenhuma informação encontrada para a chave '{key}'.";
        
        return $"📋 **{key}:** {value}";
    }

    /// <summary>
    /// Clears all context for a session
    /// </summary>
    public string ClearContext(string sessionToken)
    {
        _logger.LogInformation("Clearing context for session {Session}", sessionToken);
        
        _contexts.Remove(sessionToken);
        
        return $"🧹 Contexto da conversa limpo com sucesso.";
    }

    /// <summary>
    /// Gets a summary of the current conversation context
    /// </summary>
    public string GetConversationSummary(string sessionToken)
    {
        _logger.LogInformation("Getting conversation summary for session {Session}", sessionToken);
        
        if (!_contexts.TryGetValue(sessionToken, out var context) || !context.Any())
            return $"📭 Nenhuma informação no contexto da conversa.\n\nO contexto é usado para armazenar informações como:\n" +
                   $"- Nome do cliente\n" +
                   $"- Endereço de entrega\n" +
                   $"- Preferências de pizza";
        
        var summary = string.Join("\n", context.Select(kvp => $"📝 **{kvp.Key}:** {kvp.Value}"));
        return $"📋 **Resumo da Conversa**\n\n{summary}";
    }

    /// <summary>
    /// Sets the delivery address for a session
    /// </summary>
    public string SetDeliveryAddress(string sessionToken, string address)
    {
        _logger.LogInformation("Setting delivery address for session {Session}", sessionToken);
        
        return SetContext(sessionToken, "delivery_address", address);
    }

    /// <summary>
    /// Gets the delivery address for a session
    /// </summary>
    public string GetDeliveryAddress(string sessionToken)
    {
        return GetContext(sessionToken, "delivery_address");
    }

    /// <summary>
    /// Gets session information
    /// </summary>
    public string GetSessionInfo(string sessionToken)
    {
        _logger.LogInformation("Getting session info for {Session}", sessionToken);
        
        if (!_contexts.TryGetValue(sessionToken, out var context))
            return $"📋 **Informações da Sessão**\n\n" +
                   $"🆔 **Token:** {sessionToken}\n" +
                   $"📭 Nenhuma informação armazenada.";
        
        return $"📋 **Informações da Sessão**\n\n" +
               $"🆔 **Token:** {sessionToken}\n" +
               $"📊 **Itens no contexto:** {context.Count}\n\n" +
               $"💡 Use *get_conversation_summary* para ver todos os detalhes.";
    }
}
