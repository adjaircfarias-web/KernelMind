using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for maintaining conversation context with Semantic Kernel
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
    /// Stores information in the conversation context.
    /// Use this to remember customer information like name, address, or preferences.
    /// </summary>
    [KernelFunction("set_context")]
    [Description("Stores a key-value pair in the conversation context (e.g., customer name, address)")]
    public string SetContext(
        [Description("The session token identifying this conversation")]
        string sessionToken, 
        [Description("The key/name of the information (e.g., 'customer_name', 'delivery_address')")]
        string key, 
        [Description("The value to store")]
        string value)
    {
        _logger.LogInformation("Setting context for session {Session}: {Key} = {Value}", 
            sessionToken, key, value);
        
        if (!_contexts.ContainsKey(sessionToken))
            _contexts[sessionToken] = new Dictionary<string, object>();
        
        _contexts[sessionToken][key] = value;
        
        return $"✅ Informação '{key}' armazenada com sucesso.";
    }

    /// <summary>
    /// Retrieves information from the conversation context.
    /// Use this to recall previously stored customer information.
    /// </summary>
    [KernelFunction("get_context")]
    [Description("Retrieves a value from the conversation context by key")]
    public string GetContext(
        [Description("The session token")]
        string sessionToken, 
        [Description("The key of the information to retrieve")]
        string key)
    {
        _logger.LogInformation("Getting context for session {Session}: {Key}", sessionToken, key);
        
        if (!_contexts.TryGetValue(sessionToken, out var context))
            return $"ℹ️ Nenhuma informação encontrada para a chave '{key}'.";
        
        if (!context.TryGetValue(key, out var value))
            return $"ℹ️ Nenhuma informação encontrada para a chave '{key}'.";
        
        return $"📋 **{key}:** {value}";
    }

    /// <summary>
    /// Gets a summary of all information stored in the conversation context.
    /// Use this to see what information has been collected during the conversation.
    /// </summary>
    [KernelFunction("get_conversation_summary")]
    [Description("Gets a summary of all information stored in the conversation context")]
    public string GetConversationSummary(
        [Description("The session token")]
        string sessionToken)
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
    /// Sets the delivery address for a session.
    /// Use this when the customer provides their delivery address.
    /// </summary>
    [KernelFunction("set_delivery_address")]
    [Description("Stores the delivery address for this conversation")]
    public string SetDeliveryAddress(
        [Description("The session token")]
        string sessionToken, 
        [Description("The full delivery address")]
        string address)
    {
        _logger.LogInformation("Setting delivery address for session {Session}", sessionToken);
        
        return SetContext(sessionToken, "delivery_address", address);
    }

    /// <summary>
    /// Gets the stored delivery address.
    /// Use this to recall where to deliver the order.
    /// </summary>
    [KernelFunction("get_delivery_address")]
    [Description("Retrieves the stored delivery address")]
    public string GetDeliveryAddress(
        [Description("The session token")]
        string sessionToken)
    {
        return GetContext(sessionToken, "delivery_address");
    }

    /// <summary>
    /// Sets the customer name for a session.
    /// Use this when the customer tells you their name.
    /// </summary>
    [KernelFunction("set_customer_name")]
    [Description("Stores the customer name for this conversation")]
    public string SetCustomerName(
        [Description("The session token")]
        string sessionToken, 
        [Description("The customer's name")]
        string name)
    {
        _logger.LogInformation("Setting customer name for session {Session}", sessionToken);
        
        return SetContext(sessionToken, "customer_name", name);
    }

    /// <summary>
    /// Gets the stored customer name.
    /// Use this to address the customer by name.
    /// </summary>
    [KernelFunction("get_customer_name")]
    [Description("Retrieves the stored customer name")]
    public string GetCustomerName(
        [Description("The session token")]
        string sessionToken)
    {
        return GetContext(sessionToken, "customer_name");
    }

    /// <summary>
    /// Clears all context for a session.
    /// Use this when starting a fresh conversation.
    /// </summary>
    [KernelFunction("clear_context")]
    [Description("Clears all stored information from the conversation context")]
    public string ClearContext(
        [Description("The session token")]
        string sessionToken)
    {
        _logger.LogInformation("Clearing context for session {Session}", sessionToken);
        
        _contexts.Remove(sessionToken);
        
        return $"🧹 Contexto da conversa limpo com sucesso.";
    }
}
