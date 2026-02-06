using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for order management
/// </summary>
public class OrderPlugin
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderPlugin> _logger;
    private readonly Dictionary<string, Order> _orders = new();

    public OrderPlugin(
        IPizzaRepository pizzaRepository,
        IOrderRepository orderRepository,
        ILogger<OrderPlugin> logger)
    {
        _pizzaRepository = pizzaRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order for a customer
    /// </summary>
    public async Task<string> CreateOrderAsync(
        string customerName,
        string address,
        string phone = "",
        CancellationToken ct = default)
    {
        _logger.LogInformation("Creating order for customer: {CustomerName}", customerName);
        
        var orderToken = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        var order = new Order
        {
            DeliveryAddress = address,
            Notes = $"Customer: {customerName}, Phone: {phone}",
            Status = OrderStatus.Pending
        };
        
        _orders[orderToken] = order;
        
        return $"✅ **Pedido Criado!**\n\n" +
               $"📋 **Número do Pedido:** **{orderToken}**\n" +
               $"👤 **Cliente:** {customerName}\n" +
               $"📍 **Endereço:** {address}\n" +
               $"📞 **Telefone:** {phone}\n\n" +
               $"💡 Agora você pode adicionar pizzas usando *add_item_to_order*.";
    }

    /// <summary>
    /// Adds a pizza item to an existing order
    /// </summary>
    public async Task<string> AddItemToOrderAsync(
        string orderToken,
        string pizzaName,
        int quantity = 1,
        string? notes = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Adding item to order {OrderToken}: {PizzaName} x{Quantity}", 
            orderToken, pizzaName, quantity);
        
        if (!_orders.TryGetValue(orderToken, out var order))
            return $"❌ Pedido '{orderToken}' não encontrado. Use *create_order* primeiro.";
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(pizzaName, ct);
        var pizza = pizzas.FirstOrDefault();
        
        if (pizza == null)
            return $"❌ Pizza '{pizzaName}' não encontrada. Use *list_menu* para ver as pizzas disponíveis.";
        
        var orderItem = new OrderItem
        {
            PizzaId = pizza.Id,
            Quantity = quantity,
            UnitPrice = pizza.Price,
            Notes = notes
        };
        
        order.Items.Add(orderItem);
        
        return $"✅ **Item Adicionado ao Pedido {orderToken}**\n\n" +
               $"🍕 **Pizza:** {pizza.Name}\n" +
               $"📦 **Quantidade:** {quantity}x\n" +
               $"💰 **Preço unitário:** {pizza.Price:C}\n" +
               $"📝 **Observações:** {notes ?? "Nenhuma"}\n\n" +
               $"💡 Use *calculate_order_total* para ver o valor total.";
    }

    /// <summary>
    /// Views the current order with all items
    /// </summary>
    public string ViewOrder(string orderToken)
    {
        _logger.LogInformation("Viewing order: {OrderToken}", orderToken);
        
        if (!_orders.TryGetValue(orderToken, out var order))
            return $"❌ Pedido '{orderToken}' não encontrado.";
        
        if (!order.Items.Any())
            return $"📋 **Pedido {orderToken}**\n\nO pedido está vazio. Adicione pizzas usando *add_item_to_order*.";
        
        var subtotal = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        var items = order.Items.Select(i => 
            $"🍕 **{i.Pizza.Name}** x{i.Quantity} - {i.Total:C}\n" +
            $"   📝 {i.Notes ?? "Nenhuma"}");
        
        return $"📋 **Pedido {orderToken}**\n\n" +
               $"**Itens do Pedido:**\n" +
               string.Join("\n\n", items) + 
               $"\n\n💰 **Subtotal:** {subtotal:C}\n" +
               $"🚚 **Taxa de entrega:** R$ 5,00\n" +
               $"**TOTAL:** {subtotal + 5m:C}";
    }

    /// <summary>
    /// Confirms the order and sends it to the kitchen
    /// </summary>
    public async Task<string> ConfirmOrderAsync(
        string orderToken,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Confirming order: {OrderToken}", orderToken);
        
        if (!_orders.TryGetValue(orderToken, out var order))
            return $"❌ Pedido '{orderToken}' não encontrado.";
        
        if (!order.Items.Any())
            return $"⚠️ Pedido '{orderToken}' está vazio. Adicione itens antes de confirmar.";
        
        var confirmedOrder = order with { Status = OrderStatus.Confirmed };
        
        await _orderRepository.CreateAsync(confirmedOrder, ct);
        
        return $"✅ **Pedido {orderToken} Confirmado!** 🎉\n\n" +
               $"O seu pedido foi enviado para a cozinha!\n\n" +
               $"⏱️ **Tempo estimado:** 30-45 minutos\n" +
               $"📍 **Entrega:** {order.DeliveryAddress}\n\n" +
               $"Obrigado pela preferência! 🍕";
    }

    /// <summary>
    /// Cancels an order if it hasn't been prepared yet
    /// </summary>
    public string CancelOrder(string orderToken)
    {
        _logger.LogInformation("Cancelling order: {OrderToken}", orderToken);
        
        if (!_orders.Remove(orderToken))
            return $"❌ Pedido '{orderToken}' não encontrado ou já foi cancelado.";
        
        return $"❌ **Pedido {orderToken} Cancelado**\n\n" +
               $"O seu pedido foi cancelado conforme solicitado.\n" +
               $"Se precisar, use *create_order* para fazer um novo pedido.";
    }
}
