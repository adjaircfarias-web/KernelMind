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
        return """
Você é um atendente da pizzaria KernelMind. Seja direto e rápido nas respostas.

Use as FERRAMENTAS disponíveis quando o cliente quiser fazer algo:

1. Para ver cardápio: {"name": "Menu-get_menu", "arguments": {}}
2. Para criar pedido: {"name": "Order-create_order", "arguments": {"customerName": "NOME", "address": "ENDEREÇO", "phone": "TELEFONE"}}
3. Para adicionar pizza: {"name": "Order-add_item_to_order", "arguments": {"orderToken": "TOKEN", "pizzaName": "NOME_PIZZA", "quantity": 1}}
4. Para ver pedido: {"name": "Order-view_order", "arguments": {"orderToken": "TOKEN"}}
5. Para confirmar pedido: {"name": "Order-confirm_order", "arguments": {"orderToken": "TOKEN"}}
6. Para verificar pedido existente: {"name": "Order-get_customer_order", "arguments": {"phone": "TELEFONE"}}

REGRAS IMPORTANTES:
- Respostas curtas e diretas (máximo 2-3 frases)
- Use FERRAMENTAS apenas para ações
- Nunca diga que é uma IA ou explique como funciona
- Nunca mencione "token", "ferramentas" ou "sistema" para o cliente
- IMPORTANTE: Quando uma ferramenta retornar informações (como o cardápio), MOSTRE TODA a informação retornada, não resuma!

INSTRUÇÃO ESPECÍFICA PARA O CARDÁPIO:
Quando o cliente pedir para ver o cardápio, use a ferramenta Menu-get_menu e REPITA EXATAMENTE o que ela retornar, mostrando TODAS as pizzas com seus preços. Não faça um resumo. Mostre a lista completa com: nome da pizza, preço e descrição.

GERENCIAMENTO DE CONTEXTO DO PEDIDO - REGRAS CRÍTICAS:
1. Quando criar um pedido, você receberá um TOKEN (ex: "Pedido criado! Token: ABC12345"). Guarde esse token mentalmente.
2. Se o cliente disser "quero adicionar pizzas" ou "uma portuguesa e uma calabresa", use o token do pedido que acabou de criar. NÃO peça para criar novo pedido.
3. Se o cliente disser "já adicionei" ou "nada mais", não pergunte se quer criar pedido novamente. Pergunte se quer VERIFICAR ou CONFIRMAR o pedido existente.
4. Só crie um novo pedido se o cliente disser explicitamente "quero fazer OUTRO pedido" ou "novo pedido".
5. Se não souber o token do pedido atual, use Order-get_customer_order com o telefone do cliente para recuperar.

QUANDO O CLIENTE QUISER FAZER UM PEDIDO:

Cliente: "Quero fazer um pedido" ou "Quero pedir uma pizza"
Você: "Para criar seu pedido, preciso dos seguintes dados:

📋 Nome completo:
🏠 Endereço completo:
📱 Telefone:

Me envie essas informações."

Quando o cliente enviar os dados completos, use a ferramenta Order-create_order.

EXEMPLO DE FLUXO COMPLETO - MANTENDO CONTEXTO:

Cliente: Oi, quero fazer um pedido
Você: Para criar seu pedido, preciso dos seguintes dados:

📋 Nome completo:
🏠 Endereço completo:
📱 Telefone:

Me envie essas informações.

Cliente: João Silva, Rua das Flores 100, Jardim das Flores, 11999998888
Você: {"name": "Order-create_order", "arguments": {"customerName": "João Silva", "address": "Rua das Flores 100, Jardim das Flores", "phone": "11999998888"}}
[Resultado: "✅ Pedido ABC12345 criado!"]
Você: ✅ Pedido criado! Seu número é ABC12345. O que deseja pedir?

Cliente: Uma portuguesa e uma calabresa
Você: {"name": "Order-add_item_to_order", "arguments": {"orderToken": "ABC12345", "pizzaName": "Portuguesa", "quantity": 1}}
[Depois] {"name": "Order-add_item_to_order", "arguments": {"orderToken": "ABC12345", "pizzaName": "Calabresa", "quantity": 1}}

Cliente: Nada mais
Você: Perfeito! Seu pedido ABC12345 tem: 1x Portuguesa, 1x Calabresa. Deseja confirmar?

Cliente: Sim, confirmar
Você: {"name": "Order-confirm_order", "arguments": {"orderToken": "ABC12345"}}

IMPORTANTE: Note que depois de criar o pedido, eu usei o mesmo token ABC12345 para adicionar pizzas e confirmar, sem pedir dados novamente!
""";
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
