using KernelMind.Domain.Entities;
using KernelMind.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for order management
/// </summary>
public class OrderPlugin
{
    private readonly ILogger<OrderPlugin> _logger;
    private readonly Dictionary<string, Order> _orders = new();

    public OrderPlugin(ILogger<OrderPlugin> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order for a customer
    /// </summary>
    public string CreateOrder(string customerName, string address)
    {
        _logger.LogInformation("Creating order for customer: {CustomerName}", customerName);
        
        var orderToken = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        _orders[orderToken] = new Order(); // Placeholder
        
        return $"Pedido criado com sucesso! Número do pedido: **{orderToken}**\n" +
               $"Cliente: {customerName}\n" +
               $"Endereço: {address}\n\n" +
               $"Agora você pode adicionar itens.";
    }

    /// <summary>
    /// Adds a pizza item to an existing order
    /// </summary>
    public string AddItemToOrder(string orderToken, string pizzaName, int quantity = 1, string? notes = null)
    {
        _logger.LogInformation("Adding item to order {OrderToken}: {PizzaName} x{Quantity}", 
            orderToken, pizzaName, quantity);
        
        return $"Adicionado ao pedido **{orderToken}**:\n" +
               $"- {quantity}x {pizzaName}\n" +
               $"{(!string.IsNullOrEmpty(notes) ? $"Observações: {notes}" : "")}\n\n" +
               $"Use calculate_order_total para ver o valor total.";
    }

    /// <summary>
    /// Confirms the order and sends it to the kitchen
    /// </summary>
    public string ConfirmOrder(string orderToken)
    {
        _logger.LogInformation("Confirming order: {OrderToken}", orderToken);
        
        return $"✅ Pedido **{orderToken}** confirmado com sucesso!\n\n" +
               $"Seu pedido foi enviado para a cozinha.\n" +
               $"Tempo estimado de preparo: 30-45 minutos.\n" +
               $"Agradecemos a preferência!";
    }

    /// <summary>
    /// Cancels an order if it hasn't been prepared yet
    /// </summary>
    public string CancelOrder(string orderToken)
    {
        _logger.LogInformation("Cancelling order: {OrderToken}", orderToken);
        
        return $"❌ Pedido **{orderToken}** cancelado.\n\n" +
               $"Seu pedido foi cancelado conforme solicitado.";
    }
}
