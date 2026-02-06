using KernelMind.Core.Plugins;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Services;

/// <summary>
/// Service that orchestrates chat interactions using Ollama
/// </summary>
public class ChatService : IKernelService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<ChatService> _logger;
    private readonly MenuPlugin _menuPlugin;
    private readonly OrderPlugin _orderPlugin;
    private readonly CalculationPlugin _calculationPlugin;
    private readonly ContextPlugin _contextPlugin;

    public ChatService(
        IChatClient chatClient,
        ILogger<ChatService> logger,
        MenuPlugin menuPlugin,
        OrderPlugin orderPlugin,
        CalculationPlugin calculationPlugin,
        ContextPlugin contextPlugin)
    {
        _chatClient = chatClient;
        _logger = logger;
        _menuPlugin = menuPlugin;
        _orderPlugin = orderPlugin;
        _calculationPlugin = calculationPlugin;
        _contextPlugin = contextPlugin;
    }

    public IChatClient ChatClient => _chatClient;

    /// <summary>
    /// Processes a user message and returns the assistant response
    /// </summary>
    public async Task<string> ProcessMessageAsync(
        string sessionId, 
        string message, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Processing message for session {SessionId}: {Message}", sessionId, message);

        try
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, GetSystemPrompt()),
                new ChatMessage(ChatRole.User, message)
            };

            var response = await _chatClient.CompleteAsync(messages, cancellationToken: ct);

            var responseText = response.Message.Text ?? "Desculpe, não consegui processar sua mensagem.";
            
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
    /// Streams the chat response using IAsyncEnumerable
    /// </summary>
    public async IAsyncEnumerable<string> StreamMessageAsync(
        string sessionId,
        string message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        _logger.LogInformation("Streaming message for session {SessionId}: {Message}", sessionId, message);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, GetSystemPrompt()),
            new ChatMessage(ChatRole.User, message)
        };

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: ct))
        {
            if (ct.IsCancellationRequested)
                yield break;

            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }

    private string GetSystemPrompt()
    {
        return @"Você é um assistente virtual de uma pizzaria chamada KernelMind. Seu objetivo é ajudar os clientes a fazerem pedidos de forma natural e conversacional.

CAPACIDADES:
- Consultar o cardápio completo
- Fornecer detalhes sobre pizzas específicas (ingredientes, preços)
- Buscar pizzas por ingredientes ou descrição
- Criar novos pedidos
- Adicionar itens a pedidos existentes
- Calcular totais com taxa de entrega
- Aplicar cupons de desconto
- Confirmar ou cancelar pedidos

INSTRUÇÕES:
1. Seja sempre cordial e profissional
2. Responda em português brasileiro
3. Use emojis ocasionalmente para tornar a conversa mais amigável 
4. Quando o cliente quiser ver o cardápio, forneça informações sobre as pizzas disponíveis
5. Quando o cliente mencionar uma pizza específica, dê detalhes sobre ela
6. Para criar um pedido, peça o nome do cliente e endereço de entrega
7. Sempre confirme os detalhes antes de finalizar um pedido
8. O tempo médio de entrega é de 30-45 minutos
9. A taxa de entrega é de R$ 5,00

HORÁRIO DE FUNCIONAMENTO:
- Todos os dias das 18:00 às 23:00
- Fins de semana das 17:00 às 23:00

FORMAS DE PAGAMENTO:
- Dinheiro
- Cartões de crédito e débito
- Pix
- Carteiras digitais (Apple Pay, Google Pay)

Se não souber algo ou não puder ajudar com uma solicitação específica, seja honesto e direcione o cliente para falar com um atendente humano.";
    }
}
