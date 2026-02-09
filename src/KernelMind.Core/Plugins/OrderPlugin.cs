using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for order management with Semantic Kernel
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
    /// Creates a new order for a customer.
    /// Use this when the customer wants to start a new order.
    /// </summary>
    [KernelFunction("create_order")]
    [Description("Creates a new order for a customer and returns an order token")]
    public async Task<string> CreateOrderAsync(
        [Description("Customer's full name")]
        string customerName,
        [Description("Delivery address")]
        string address,
        [Description("Customer's phone number (optional)")]
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
    /// Adds a pizza item to an existing order.
    /// Use this when the customer wants to add a pizza to their order.
    /// </summary>
    [KernelFunction("add_item_to_order")]
    [Description("Adds a pizza item to an existing order")]
    public async Task<string> AddItemToOrderAsync(
        [Description("The order token (received from create_order)")]
        string orderToken,
        [Description("Exact name of the pizza to add")]
        string pizzaName,
        [Description("Quantity of pizzas (default: 1)")]
        int quantity = 1,
        [Description("Any special notes or requests (optional)")]
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
            return $"❌ Pizza '{pizzaName}' não encontrada. Use *get_menu* para ver as pizzas disponíveis.";
        
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
               $"💡 Use *view_order* para ver o pedido completo.";
    }

    /// <summary>
    /// Views the current order with all items.
    /// Use this to show the customer what's in their order.
    /// </summary>
    [KernelFunction("view_order")]
    [Description("Views the current order with all items and total price")]
    public string ViewOrder(
        [Description("The order token")]
        string orderToken)
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
    /// Confirms the order and sends it to the kitchen.
    /// Use this when the customer is ready to finalize their order.
    /// </summary>
    [KernelFunction("confirm_order")]
    [Description("Confirms the order and sends it to the kitchen for preparation")]
    public async Task<string> ConfirmOrderAsync(
        [Description("The order token to confirm")]
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
    /// Removes an item from an existing order.
    /// Use this when the customer wants to remove something from their order.
    /// </summary>
    [KernelFunction("remove_item_from_order")]
    [Description("Removes an item from an existing order by its index")]
    public string RemoveItemFromOrder(
        [Description("The order token")]
        string orderToken, 
        [Description("The index of the item to remove (0-based)")]
        int itemIndex)
    {
        _logger.LogInformation("Removing item from order {OrderToken}: Item {Index}", 
            orderToken, itemIndex);
        
        if (!_orders.TryGetValue(orderToken, out var order))
            return $"❌ Pedido '{orderToken}' não encontrado.";
        
        if (itemIndex < 0 || itemIndex >= order.Items.Count)
            return $"❌ Item {itemIndex + 1} não encontrado no pedido.";
        
        var removedItem = order.Items[itemIndex];
        order.Items.RemoveAt(itemIndex);
        
        return $"✅ **Item Removido do Pedido {orderToken}**\n\n" +
               $"🍕 **Pizza removida:** {removedItem.Pizza?.Name ?? "Unknown"}\n" +
               $"📦 **Quantidade:** {removedItem.Quantity}x\n\n" +
               $"💡 Use *view_order* para ver o pedido atualizado.";
    }

    /// <summary>
    /// Cancels an order if it hasn't been prepared yet.
    /// Use this when the customer wants to cancel their order.
    /// </summary>
    [KernelFunction("cancel_order")]
    [Description("Cancels an order if it hasn't been prepared yet")]
    public string CancelOrder(
        [Description("The order token to cancel")]
        string orderToken)
    {
        _logger.LogInformation("Cancelling order: {OrderToken}", orderToken);
        
        if (!_orders.Remove(orderToken))
            return $"❌ Pedido '{orderToken}' não encontrado ou já foi cancelado.";
        
        return $"❌ **Pedido {orderToken} Cancelado**\n\n" +
               $"O seu pedido foi cancelado conforme solicitado.\n" +
               $"Se precisar, use *create_order* para fazer um novo pedido.";
    }

    /// <summary>
    /// Gets tracking information for an order.
    /// Use this when the customer asks about their order status.
    /// </summary>
    [KernelFunction("get_order_tracking")]
    [Description("Gets tracking information and status for an order")]
    public string GetOrderTracking(
        [Description("The order token")]
        string orderToken)
    {
        _logger.LogInformation("Getting tracking for order: {OrderToken}", orderToken);
        
        if (!_orders.TryGetValue(orderToken, out var order))
            return $"📍 **Rastreamento do Pedido {orderToken}**\n\n" +
                   $"⚠️ Pedido não encontrado no sistema atual.\n\n" +
                   $"💡 Verifique se o número do pedido está correto.";
        
        var status = order.Status switch
        {
            OrderStatus.Pending => "⏳ Pendente - Aguardando confirmação",
            OrderStatus.Confirmed => "✅ Confirmado - Enviado para a cozinha",
            OrderStatus.Preparing => "👨‍🍳 Em preparação",
            OrderStatus.Ready => "✨ Pronto para entrega",
            OrderStatus.OutForDelivery => "🛵 Saiu para entrega",
            OrderStatus.Delivered => "✅ Entregue",
            OrderStatus.Cancelled => "❌ Cancelado",
            _ => "⏳ Status desconhecido"
        };
        
        return $"📍 **Rastreamento do Pedido {orderToken}**\n\n" +
               $"{status}\n\n" +
               $"📍 **Endereço:** {order.DeliveryAddress}\n" +
               $"⏱️ **Tempo estimado:** 30-45 minutos\n\n" +
               $"📞 Você receberá uma ligação para confirmar a entrega.";
    }
}
