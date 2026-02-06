namespace KernelMind.Api.DTOs;

public record ChatRequest
{
    public string? SessionId { get; init; }
    public required string Message { get; init; }
}

public record ChatResponse
{
    public required string Response { get; init; }
    public required string SessionId { get; init; }
    public DateTime Timestamp { get; init; }
}
