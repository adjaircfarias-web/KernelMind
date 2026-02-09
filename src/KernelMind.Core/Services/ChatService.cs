using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using KernelMind.Domain.Interfaces;
using KernelMind.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Services;

/// <summary>
/// Service that orchestrates chat interactions using Semantic Kernel with Function Calling
/// </summary>
public class ChatService
{
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
            
            // Build chat history from database
            var chatHistory = new ChatHistory(GetSystemPrompt());
            
            foreach (var msg in chatSession.Messages.OrderBy(m => m.CreatedAt))
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
            
            // Enable function calling
            var executionSettings = new OllamaPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // Get response with potential function calls
            var response = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                _kernel,
                ct);

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
        
        // Load conversation history
        var chatSession = await GetOrCreateSessionAsync(sessionId, ct);
        foreach (var msg in chatSession.Messages.OrderBy(m => m.CreatedAt))
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

        var responseBuilder = new System.Text.StringBuilder();

        await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory,
            executionSettings,
            _kernel,
            ct))
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

    private string GetSystemPrompt()
    {
        return @"Você é um assistente virtual de uma pizzaria chamada KernelMind. Seu objetivo é ajudar os clientes a fazerem pedidos de forma natural e conversacional.

CAPACIDADES DISPONÍVEIS (use as funções quando apropriado):

🍕 **Cardápio:**
- get_menu: Lista todas as pizzas disponíveis
- get_pizza_details: Mostra detalhes de uma pizza específica
- search_pizzas: Busca pizzas por ingredientes ou nome

📦 **Pedidos:**
- create_order: Cria um novo pedido (precisa de nome e endereço)
- add_item_to_order: Adiciona uma pizza ao pedido
- view_order: Mostra o pedido atual
- confirm_order: Confirma e envia o pedido para a cozinha
- cancel_order: Cancela um pedido
- get_order_tracking: Mostra o status do pedido

💰 **Cálculos:**
- calculate_total: Calcula o total com taxa de entrega
- calculate_delivery_fee: Calcula taxa baseada na distância
- apply_discount: Aplica cupom de desconto
- check_promotion: Mostra a promoção do dia
- split_bill: Divide a conta entre pessoas

📝 **Contexto:**
- set_context: Armazena informações (nome, endereço, etc.)
- get_context: Recupera informações armazenadas
- set_customer_name: Armazena o nome do cliente
- get_customer_name: Recupera o nome do cliente
- set_delivery_address: Armazena o endereço de entrega
- get_delivery_address: Recupera o endereço

INSTRUÇÕES IMPORTANTES:
1. **MEMÓRIA**: Use as funções de contexto para lembrar informações do cliente (nome, endereço). NÃO pergunte novamente o que já foi informado!

2. **CONTEXTO DA CONVERSA**: Você tem acesso ao histórico completo da conversa. Use essas informações para personalizar o atendimento.

3. **FUNÇÕES**: Quando o cliente quiser fazer algo (ver cardápio, criar pedido, calcular preço), CHAME A FUNÇÃO apropriada. Não apenas descreva o que faria.

4. **CRIAÇÃO DE PEDIDO**: Sempre que criar um pedido, armazene o token do pedido no contexto para referência futura.

5. **CONFIRMAÇÃO**: Antes de confirmar um pedido, mostre todos os itens e peça confirmação explícita.

INFORMAÇÕES DO ESTABELECIMENTO:
- Tempo médio de entrega: 30-45 minutos
- Taxa de entrega: R$ 5,00 (padrão)
- Horário: Todos os dias 18h-23h, Fins de semana 17h-23h
- Pagamento: Dinheiro, cartão, Pix, carteiras digitais

Seja sempre cordial, use emojis ocasionalmente, e responda em português brasileiro.";
    }

    private string GetSystemPromptForStreaming()
    {
        return @"Você é um assistente virtual de uma pizzaria chamada KernelMind.

IMPORTANTE: No modo de streaming você NÃO pode usar funções/tools. Responda com base apenas nas informações que você já conhece ou que foram fornecidas no histórico da conversa.

Para operações que exigem consulta ao cardápio, criação de pedidos, ou cálculos precisos, informe ao cliente que ele deve usar o modo de resposta completa (não streaming).

INFORMAÇÕES DO ESTABELECIMENTO:
- Tempo médio de entrega: 30-45 minutos
- Taxa de entrega: R$ 5,00 (padrão)
- Horário: Todos os dias 18h-23h, Fins de semana 17h-23h
- Pagamento: Dinheiro, cartão, Pix, carteiras digitais

Seja sempre cordial, use emojis ocasionalmente, e responda em português brasileiro.";
    }
}
