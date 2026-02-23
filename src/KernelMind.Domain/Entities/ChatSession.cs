namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a chat session with the bot
/// </summary>
public record ChatSession
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid? CustomerId { get; init; }
    public Customer? Customer { get; init; }
    public string SessionToken { get; init; } = Guid.NewGuid().ToString();
    public Dictionary<string, object> Context { get; init; } = new();
    public bool IsActive { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; init; } = DateTime.UtcNow;
    
    public List<ChatMessage> Messages { get; init; } = new();
}
