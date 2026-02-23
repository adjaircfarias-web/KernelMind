namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a pizza in the menu
/// </summary>
public record Pizza
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Ingredients { get; set; } = new();
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// Vector embedding for semantic search (stored in PostgreSQL pgvector)
    public float[]? Embedding { get; set; }
}
