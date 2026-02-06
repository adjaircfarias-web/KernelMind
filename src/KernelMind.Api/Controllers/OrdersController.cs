using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KernelMind.Api.Controllers;

/// <summary>
/// API controller for order management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderRepository orderRepository,
        IPizzaRepository pizzaRepository,
        ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _pizzaRepository = pizzaRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(bool includeItems = true, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting all orders");
        var orders = await _orderRepository.GetAllAsync(ct);
        return Ok(orders.Select(o => OrderDto.FromEntity(o)));
    }

    /// <summary>
    /// Gets a specific order by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting order: {OrderId}", id);
        var order = await _orderRepository.GetByIdAsync(id, ct);
        
        if (order == null)
            return NotFound(new { error = "Order not found" });
        
        return Ok(OrderDto.FromEntity(order));
    }

    /// <summary>
    /// Gets orders by customer ID
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId, CancellationToken ct)
    {
        _logger.LogInformation("Getting orders for customer: {CustomerId}", customerId);
        var orders = await _orderRepository.GetByCustomerAsync(customerId, ct);
        return Ok(orders.Select(o => OrderDto.FromEntity(o)));
    }

    /// <summary>
    /// Gets orders by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(string status, CancellationToken ct)
    {
        _logger.LogInformation("Getting orders by status: {Status}", status);
        if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            return BadRequest(new { error = "Invalid status" });
        
        var orders = await _orderRepository.GetByStatusAsync(orderStatus, ct);
        return Ok(orders.Select(o => OrderDto.FromEntity(o)));
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating new order for customer: {CustomerName}", request.CustomerName);
        
        if (request.Items == null || !request.Items.Any())
            return BadRequest(new { error = "Order must have at least one item" });

        var items = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            var pizza = await _pizzaRepository.GetByIdAsync(item.PizzaId, ct);
            if (pizza == null)
                return BadRequest(new { error = $"Pizza with ID {item.PizzaId} not found" });

            items.Add(new OrderItem
            {
                PizzaId = pizza.Id,
                Quantity = item.Quantity,
                UnitPrice = pizza.Price,
                Notes = item.Notes
            });
        }

        var subtotal = items.Sum(i => i.Quantity * i.UnitPrice);
        const decimal deliveryFee = 5.00m;

        var order = new Order
        {
            CustomerId = request.CustomerId ?? Guid.Empty,
            DeliveryAddress = request.DeliveryAddress ?? "",
            Notes = request.Notes,
            Status = OrderStatus.Pending,
            Items = items,
            TotalAmount = subtotal + deliveryFee
        };

        var created = await _orderRepository.CreateAsync(order, ct);
        _logger.LogInformation("Order created: {OrderId}", created.Id);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, OrderDto.FromEntity(created));
    }

    /// <summary>
    /// Updates order status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Updating order status: {OrderId} to {Status}", id, request.Status);
        
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order == null)
            return NotFound(new { error = "Order not found" });

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
            return BadRequest(new { error = "Invalid status" });

        var updatedOrder = order with { Status = newStatus };
        await _orderRepository.UpdateAsync(updatedOrder, ct);
        
        return Ok(OrderDto.FromEntity(updatedOrder));
    }

    /// <summary>
    /// Cancels an order
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderDto>> CancelOrder(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Cancelling order: {OrderId}", id);
        
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order == null)
            return NotFound(new { error = "Order not found" });

        if (order.Status == OrderStatus.Preparing || order.Status == OrderStatus.OutForDelivery || order.Status == OrderStatus.Delivered)
            return BadRequest(new { error = "Cannot cancel an order that is already being prepared or delivered" });

        var cancelledOrder = order with { Status = OrderStatus.Cancelled };
        await _orderRepository.UpdateAsync(cancelledOrder, ct);
        
        return Ok(OrderDto.FromEntity(cancelledOrder));
    }
}

/// <summary>
/// Request DTOs
/// </summary>
public record CreateOrderRequest(
    Guid? CustomerId,
    string? CustomerName,
    string? DeliveryAddress,
    string? Phone,
    string? Notes,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
    Guid PizzaId,
    int Quantity,
    string? Notes
);

public record UpdateStatusRequest(string Status);

/// <summary>
/// Response DTOs
/// </summary>
public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    string? DeliveryAddress,
    string? Notes,
    List<OrderItemDto>? Items,
    DateTime CreatedAt
)
{
    public static OrderDto FromEntity(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.TotalAmount,
        order.DeliveryAddress,
        order.Notes,
        order.Items?.Select(i => OrderItemDto.FromEntity(i)).ToList(),
        order.CreatedAt
    );
}

public record OrderItemDto(
    Guid Id,
    Guid PizzaId,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    string? Notes
)
{
    public static OrderItemDto FromEntity(OrderItem item) => new(
        item.Id,
        item.PizzaId,
        item.Quantity,
        item.UnitPrice,
        item.Total,
        item.Notes
    );
}
