namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a customer
/// </summary>
public record Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    
    public List<Order> Orders { get; init; } = new();
}
