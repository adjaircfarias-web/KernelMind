using KernelMind.Domain.Entities;

namespace KernelMind.Domain.Interfaces;

/// <summary>
/// Repository interface for ChatSession entity
/// </summary>
public interface IChatSessionRepository
{
    Task<ChatSession?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ChatSession>> GetActiveAsync(CancellationToken ct = default);
    Task<ChatSession> CreateAsync(ChatSession session, CancellationToken ct = default);
    Task UpdateAsync(ChatSession session, CancellationToken ct = default);
    Task AddMessageAsync(ChatMessage message, CancellationToken ct = default);
}
