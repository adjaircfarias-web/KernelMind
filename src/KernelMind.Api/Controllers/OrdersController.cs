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
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Getting all orders");
        var orders = await _orderRepository.GetAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Gets a specific order by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting order: {OrderId}", id);
        var order = await _orderRepository.GetByIdAsync(id, ct);
        
        if (order == null)
            return NotFound();
        
        return Ok(order);
    }

    /// <summary>
    /// Gets orders by customer ID
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetByCustomer(
        Guid customerId, 
        CancellationToken ct)
    {
        _logger.LogInformation("Getting orders for customer: {CustomerId}", customerId);
        var orders = await _orderRepository.GetByCustomerAsync(customerId, ct);
        return Ok(orders);
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(
        [FromBody] CreateOrderRequest request, 
        CancellationToken ct)
    {
        _logger.LogInformation("Creating new order for customer: {CustomerName}", request.CustomerName);
        
        // In a real implementation, this would properly create the order
        var order = new Order
        {
            Customer = new Customer { Name = request.CustomerName },
            DeliveryAddress = request.Address,
            Status = OrderStatus.Pending
        };
        
        var created = await _orderRepository.CreateAsync(order, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

public record CreateOrderRequest(string CustomerName, string Address);
