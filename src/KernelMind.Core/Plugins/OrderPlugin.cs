using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for order management with Semantic Kernel - Database persistence
/// </summary>
public class OrderPlugin
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<OrderPlugin> _logger;

    public OrderPlugin(
        IPizzaRepository pizzaRepository,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        ILogger<OrderPlugin> logger)
    {
        _pizzaRepository = pizzaRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets the most recent active order for a customer by phone.
    /// Use this to check if the customer already has an active order before creating a new one.
    /// </summary>
    [KernelFunction("get_customer_order")]
    [Description("Gets the customer's most recent active order by phone number")]
    public async Task<string> GetCustomerOrderAsync(
        [Description("Customer's phone number")]
        string phone,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting active order for phone: {Phone}", phone);
        
        try
        {
            // Find customer by phone
            var customer = await _customerRepository.GetByPhoneAsync(phone, ct);
            if (customer == null)
            {
                return "Nenhum pedido encontrado para este telefone.";
            }
            
            // Get customer's most recent pending order
            var orders = await _orderRepository.GetByCustomerAsync(customer.Id, ct);
            var activeOrder = orders
                .Where(o => o.Status == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
            
            if (activeOrder == null)
            {
                return "Nenhum pedido ativo encontrado.";
            }
            
            // Extract token from notes
            var token = activeOrder.Notes?.Split("Token: ").LastOrDefault()?.Split(" ").FirstOrDefault() ?? "N/A";
            var items = await _orderRepository.GetOrderItemsAsync(activeOrder.Id, ct);
            var itemList = items.Any() ? string.Join(", ", items.Select(i => $"{i.Quantity}x {i.Pizza?.Name}")) : "Nenhuma pizza adicionada";
            
            return $"Pedido encontrado! Token: {token} | Itens: {itemList}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer order for phone: {Phone}", phone);
            return "Erro ao buscar pedido.";
        }
    }

    /// <summary>
    /// Creates a new order for a customer and saves it to the database.
    /// Use this when the customer wants to start a new order.
    /// </summary>
    [KernelFunction("create_order")]
    [Description("Creates a new order for a customer, saves to database, and returns an order token")]
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
        
        try
        {
            // Try to find existing customer by phone, or create new one
            Customer? customer = null;
            if (!string.IsNullOrEmpty(phone))
            {
                customer = await _customerRepository.GetByPhoneAsync(phone, ct);
            }
            
            if (customer == null)
            {
                try
                {
                    customer = new Customer
                    {
                        Name = customerName,
                        Phone = phone,
                        Address = address
                    };
                    await _customerRepository.CreateAsync(customer, ct);
                    _logger.LogInformation("Created new customer: {CustomerName} with ID: {CustomerId}", customerName, customer.Id);
                }
                catch (Exception ex) when (ex.InnerException?.Message?.Contains("23505") == true || 
                                           ex.Message?.Contains("duplicate") == true ||
                                           ex.Message?.Contains("unique constraint") == true)
                {
                    // Customer was created by another concurrent request, find it by phone
                    _logger.LogWarning("Customer already exists (concurrent creation), searching by phone: {Phone}. Exception: {ExceptionMessage}", phone, ex.Message);
                    await Task.Delay(100, ct); // Small delay to allow DB to settle
                    
                    if (!string.IsNullOrEmpty(phone))
                    {
                        customer = await _customerRepository.GetByPhoneAsync(phone, ct);
                    }
                    
                    // If still not found, get the most recent customer
                    if (customer == null)
                    {
                        var allCustomers = await _customerRepository.GetAllAsync(ct);
                        customer = allCustomers.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
                    }
                    
                    if (customer == null)
                    {
                        throw new InvalidOperationException("Não foi possível criar ou encontrar o cliente. Por favor, tente novamente.");
                    }
                    
                    _logger.LogInformation("Using existing customer: {CustomerName} with ID: {CustomerId}", customer.Name, customer.Id);
                }
            }
            else
            {
                _logger.LogInformation("Found existing customer: {CustomerName} with ID: {CustomerId}", customer.Name, customer.Id);
            }
            
            // Generate order token
            var orderToken = Guid.NewGuid().ToString("N")[..8].ToUpper();
            
            // Create order - DON'T set Customer navigation property to avoid tracking conflicts
            var order = new Order
            {
                CustomerId = customer.Id,
                DeliveryAddress = address,
                Notes = $"Customer: {customerName}, Phone: {phone}, Token: {orderToken}",
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };
            
            // Save to database immediately
            await _orderRepository.CreateAsync(order, ct);
            _logger.LogInformation("Created order {OrderToken} with ID: {OrderId} for customer {CustomerId}", orderToken, order.Id, customer.Id);
            
            return $"✅ **Pedido Criado com Sucesso!**\n\n" +
                   $"📋 **Número do Pedido:** **{orderToken}**\n" +
                   $"👤 **Cliente:** {customerName}\n" +
                   $"📍 **Endereço:** {address}\n" +
                   $"📞 **Telefone:** {phone}\n\n" +
                   $"💡 Agora você pode adicionar pizzas usando *add_item_to_order*.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer: {CustomerName}", customerName);
            return $"❌ Erro ao criar pedido: {ex.Message}. Por favor, tente novamente.";
        }
    }

    /// <summary>
    /// Adds a pizza item to an existing order.
    /// Use this when the customer wants to add a pizza to their order.
    /// </summary>
    [KernelFunction("add_item_to_order")]
    [Description("Adds a pizza item to an existing order in the database")]
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
        
        try
        {
            // Find order by token in notes - use GetByIdAsync to avoid tracking issues with includes
            var allOrders = await _orderRepository.GetAllAsync(ct);
            var orderSummary = allOrders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (orderSummary == null)
                return $"❌ Pedido '{orderToken}' não encontrado. Use *create_order* primeiro.";
            
            // Check if order can be modified
            if (orderSummary.Status != OrderStatus.Pending)
                return $"❌ Pedido '{orderToken}' não pode ser modificado (status: {orderSummary.Status}).";
            
            var pizzas = await _pizzaRepository.SearchByNameAsync(pizzaName, ct);
            var pizza = pizzas.FirstOrDefault();
            
            if (pizza == null)
                return $"❌ Pizza '{pizzaName}' não encontrada. Use *get_menu* para ver as pizzas disponíveis.";
            
            // Create order item
            var orderItem = new OrderItem
            {
                OrderId = orderSummary.Id,
                PizzaId = pizza.Id,
                Quantity = quantity,
                UnitPrice = pizza.Price,
                Notes = notes
            };
            
            // Add item directly to database using repository method
            await _orderRepository.AddItemToOrderAsync(orderSummary.Id, orderItem, ct);
            _logger.LogInformation("Added item to order {OrderToken}: {PizzaName} x{Quantity}", orderToken, pizzaName, quantity);
            
            // Get updated items for response
            var updatedItems = await _orderRepository.GetOrderItemsAsync(orderSummary.Id, ct);
            var totalAmount = updatedItems.Sum(i => i.Quantity * i.UnitPrice);
            
            return $"✅ **Item Adicionado ao Pedido {orderToken}**\n\n" +
                   $"🍕 **Pizza:** {pizza.Name}\n" +
                   $"📦 **Quantidade:** {quantity}x\n" +
                   $"💰 **Preço unitário:** {pizza.Price:C}\n" +
                   $"📝 **Observações:** {notes ?? "Nenhuma"}\n" +
                   $"💰 **Subtotal:** {totalAmount:C}\n\n" +
                   $"💡 Use *view_order* para ver o pedido completo.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to order {OrderToken}", orderToken);
            return $"❌ Erro ao adicionar item: {ex.Message}. Por favor, tente novamente.";
        }
    }

    /// <summary>
    /// Views the current order with all items.
    /// Use this to show the customer what's in their order.
    /// </summary>
    [KernelFunction("view_order")]
    [Description("Views the current order with all items and total price from database")]
    public async Task<string> ViewOrderAsync(
        [Description("The order token")]
        string orderToken,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Viewing order: {OrderToken}", orderToken);
        
        try
        {
            // Find order by token in notes
            var orders = await _orderRepository.GetAllAsync(ct);
            var order = orders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (order == null)
                return $"❌ Pedido '{orderToken}' não encontrado.";
            
            if (!order.Items.Any())
                return $"📋 **Pedido {orderToken}**\n\nO pedido está vazio. Adicione pizzas usando *add_item_to_order*.";
            
            var subtotal = order.Items.Sum(i => i.Quantity * i.UnitPrice);
            var items = order.Items.Select(i => 
                $"🍕 **{i.Pizza?.Name ?? "Pizza"}** x{i.Quantity} - {(i.Quantity * i.UnitPrice):C}\n" +
                $"   📝 {i.Notes ?? "Nenhuma"}");
            
            return $"📋 **Pedido {orderToken}** (Status: {order.Status})\n\n" +
                   $"**Itens do Pedido:**\n" +
                   string.Join("\n\n", items) + 
                   $"\n\n💰 **Subtotal:** {subtotal:C}\n" +
                   $"🚚 **Taxa de entrega:** R$ 5,00\n" +
                   $"**TOTAL:** {subtotal + 5m:C}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing order {OrderToken}", orderToken);
            return $"❌ Erro ao visualizar pedido: {ex.Message}.";
        }
    }

    /// <summary>
    /// Confirms the order and sends it to the kitchen.
    /// Use this when the customer is ready to finalize their order.
    /// </summary>
    [KernelFunction("confirm_order")]
    [Description("Confirms the order in database and sends it to the kitchen")]
    public async Task<string> ConfirmOrderAsync(
        [Description("The order token to confirm")]
        string orderToken,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Confirming order: {OrderToken}", orderToken);
        
        try
        {
            // Find order by token in notes
            var allOrders = await _orderRepository.GetAllAsync(ct);
            var orderSummary = allOrders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (orderSummary == null)
                return $"❌ Pedido '{orderToken}' não encontrado.";
            
            // Get order items
            var items = await _orderRepository.GetOrderItemsAsync(orderSummary.Id, ct);
            
            if (!items.Any())
                return $"⚠️ Pedido '{orderToken}' está vazio. Adicione itens antes de confirmar.";
            
            if (orderSummary.Status != OrderStatus.Pending)
                return $"⚠️ Pedido '{orderToken}' já foi confirmado (status: {orderSummary.Status}).";
            
            // Get fresh order without tracking
            var order = await _orderRepository.GetByIdForUpdateAsync(orderSummary.Id, ct);
            if (order == null)
                return $"❌ Pedido '{orderToken}' não encontrado.";
            
            // Calculate final total
            var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice) + 5m; // + delivery fee
            
            // Update order status
            order = order with { Status = OrderStatus.Confirmed, TotalAmount = totalAmount };
            await _orderRepository.UpdateAsync(order, ct);
            
            _logger.LogInformation("Order {OrderToken} confirmed successfully", orderToken);
            
            return $"✅ **Pedido {orderToken} Confirmado!** 🎉\n\n" +
                   $"O seu pedido foi enviado para a cozinha!\n\n" +
                   $"📋 **Resumo:**\n" +
                   string.Join("\n", items.Select(i => $"🍕 Pizza x{i.Quantity}")) +
                   $"\n\n💰 **Total:** {totalAmount:C}\n" +
                   $"📍 **Entrega:** {order.DeliveryAddress}\n" +
                   $"⏱️ **Tempo estimado:** 30-45 minutos\n\n" +
                   $"Obrigado pela preferência! 🍕";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming order {OrderToken}", orderToken);
            return $"❌ Erro ao confirmar pedido: {ex.Message}. Por favor, tente novamente.";
        }
    }

    /// <summary>
    /// Removes an item from an existing order.
    /// Use this when the customer wants to remove something from their order.
    /// </summary>
    [KernelFunction("remove_item_from_order")]
    [Description("Removes an item from an existing order in database")]
    public async Task<string> RemoveItemFromOrderAsync(
        [Description("The order token")]
        string orderToken, 
        [Description("The index of the item to remove (0-based)")]
        int itemIndex,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Removing item from order {OrderToken}: Item {Index}", 
            orderToken, itemIndex);
        
        try
        {
            // Find order by token in notes
            var orders = await _orderRepository.GetAllAsync(ct);
            var order = orders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (order == null)
                return $"❌ Pedido '{orderToken}' não encontrado.";
            
            if (order.Status != OrderStatus.Pending)
                return $"❌ Pedido '{orderToken}' não pode ser modificado (status: {order.Status}).";
            
            if (itemIndex < 0 || itemIndex >= order.Items.Count)
                return $"❌ Item {itemIndex + 1} não encontrado no pedido.";
            
            var removedItem = order.Items[itemIndex];
            order.Items.RemoveAt(itemIndex);
            
            // Update in database
            await _orderRepository.UpdateAsync(order, ct);
            _logger.LogInformation("Removed item from order {OrderToken}", orderToken);
            
            return $"✅ **Item Removido do Pedido {orderToken}**\n\n" +
                   $"🍕 **Pizza removida:** {removedItem.Pizza?.Name ?? "Item"}\n" +
                   $"📦 **Quantidade:** {removedItem.Quantity}x\n\n" +
                   $"💡 Use *view_order* para ver o pedido atualizado.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from order {OrderToken}", orderToken);
            return $"❌ Erro ao remover item: {ex.Message}.";
        }
    }

    /// <summary>
    /// Cancels an order if it hasn't been prepared yet.
    /// Use this when the customer wants to cancel their order.
    /// </summary>
    [KernelFunction("cancel_order")]
    [Description("Cancels an order in database if it hasn't been prepared yet")]
    public async Task<string> CancelOrderAsync(
        [Description("The order token to cancel")]
        string orderToken,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Cancelling order: {OrderToken}", orderToken);
        
        try
        {
            // Find order by token in notes
            var orders = await _orderRepository.GetAllAsync(ct);
            var order = orders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (order == null)
                return $"❌ Pedido '{orderToken}' não encontrado ou já foi cancelado.";
            
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return $"❌ Pedido '{orderToken}' não pode ser cancelado (status: {order.Status}).";
            
            // Update status to cancelled
            order = order with { Status = OrderStatus.Cancelled };
            await _orderRepository.UpdateAsync(order, ct);
            _logger.LogInformation("Order {OrderToken} cancelled", orderToken);
            
            return $"❌ **Pedido {orderToken} Cancelado**\n\n" +
                   $"O seu pedido foi cancelado conforme solicitado.\n" +
                   $"Se precisar, use *create_order* para fazer um novo pedido.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderToken}", orderToken);
            return $"❌ Erro ao cancelar pedido: {ex.Message}.";
        }
    }

    /// <summary>
    /// Gets tracking information for an order.
    /// Use this when the customer asks about their order status.
    /// </summary>
    [KernelFunction("get_order_tracking")]
    [Description("Gets tracking information and status for an order from database")]
    public async Task<string> GetOrderTrackingAsync(
        [Description("The order token")]
        string orderToken,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting tracking for order: {OrderToken}", orderToken);
        
        try
        {
            // Find order by token in notes
            var orders = await _orderRepository.GetAllAsync(ct);
            var order = orders.FirstOrDefault(o => o.Notes != null && o.Notes.Contains($"Token: {orderToken}"));
            
            if (order == null)
                return $"📍 **Rastreamento do Pedido {orderToken}**\n\n" +
                       $"⚠️ Pedido não encontrado no sistema.\n\n" +
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tracking for order {OrderToken}", orderToken);
            return $"❌ Erro ao consultar pedido: {ex.Message}.";
        }
    }
}
