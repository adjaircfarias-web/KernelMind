namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a customer order
/// </summary>
public record Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public Customer Customer { get; init; } = null!;
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public decimal TotalAmount { get; init; }
    public string DeliveryAddress { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public List<OrderItem> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    Ready,
    OutForDelivery,
    Delivered,
    Cancelled
}
