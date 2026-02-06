namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents an item in an order
/// </summary>
public record OrderItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid OrderId { get; init; }
    public Order Order { get; init; } = null!;
    public Guid PizzaId { get; init; }
    public Pizza Pizza { get; init; } = null!;
    public int Quantity { get; init; } = 1;
    public decimal UnitPrice { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public decimal Total => Quantity * UnitPrice;
}
