namespace KernelMind.Domain.Entities;

/// <summary>
/// Represents a message in a chat session
/// </summary>
public record ChatMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SessionId { get; init; }
    public ChatSession Session { get; init; } = null!;
    public ChatRole Role { get; init; }
    public string Content { get; init; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public enum ChatRole
{
    System,
    User,
    Assistant
}
