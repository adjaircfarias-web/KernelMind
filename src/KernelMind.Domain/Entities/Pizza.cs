namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a pizza in the menu
/// </summary>
public record Pizza
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public List<string> Ingredients { get; init; } = new();
    public bool IsAvailable { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    // Vector embedding for semantic search (stored in PostgreSQL pgvector)
    public float[]? Embedding { get; init; }
}
