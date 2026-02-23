using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ChatSession entity
/// </summary>
public class ChatSessionRepository : IChatSessionRepository
{
    private readonly AppDbContext _context;

    public ChatSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await _context.ChatSessions
            .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(s => s.SessionToken == token && s.IsActive, ct);
    }

    public async Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IEnumerable<ChatSession>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _context.ChatSessions
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.LastActivityAt)
            .ToListAsync(ct);
    }

    public async Task<ChatSession> CreateAsync(ChatSession session, CancellationToken ct = default)
    {
        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync(ct);
        return session;
    }

    public async Task UpdateAsync(ChatSession session, CancellationToken ct = default)
    {
        _context.ChatSessions.Update(session);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddMessageAsync(ChatMessage message, CancellationToken ct = default)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync(ct);
    }
}
