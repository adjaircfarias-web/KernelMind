using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for maintaining conversation context
/// </summary>
public class ContextPlugin
{
    private readonly ILogger<ContextPlugin> _logger;
    private readonly Dictionary<string, Dictionary<string, object>> _contexts = new();

    public ContextPlugin(ILogger<ContextPlugin> logger)
    {
        _logger = logger;
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
        
        return $"Informação '{key}' armazenada com sucesso.";
    }

    /// <summary>
    /// Retrieves information from the conversation context
    /// </summary>
    public string GetContext(string sessionToken, string key)
    {
        _logger.LogInformation("Getting context for session {Session}: {Key}", sessionToken, key);
        
        if (!_contexts.TryGetValue(sessionToken, out var context))
            return $"Nenhuma informação encontrada para a chave '{key}'.";
        
        if (!context.TryGetValue(key, out var value))
            return $"Nenhuma informação encontrada para a chave '{key}'.";
        
        return $"{key}: {value}";
    }

    /// <summary>
    /// Clears all context for a session
    /// </summary>
    public string ClearContext(string sessionToken)
    {
        _logger.LogInformation("Clearing context for session {Session}", sessionToken);
        
        _contexts.Remove(sessionToken);
        
        return "Contexto da conversa limpo.";
    }

    /// <summary>
    /// Gets a summary of the current conversation context
    /// </summary>
    public string GetConversationSummary(string sessionToken)
    {
        _logger.LogInformation("Getting conversation summary for session {Session}", sessionToken);
        
        if (!_contexts.TryGetValue(sessionToken, out var context) || !context.Any())
            return "Nenhuma informação no contexto da conversa.";
        
        var summary = string.Join("\n", context.Select(kvp => $"- {kvp.Key}: {kvp.Value}"));
        return $"Resumo da conversa:\n{summary}";
    }
}
