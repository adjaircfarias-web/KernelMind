namespace KernelMind.Api.DTOs;

public record PizzaDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Ingredients { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
}

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record OrderItemDto
{
    public Guid PizzaId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
