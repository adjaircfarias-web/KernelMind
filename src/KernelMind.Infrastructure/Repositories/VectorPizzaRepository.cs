using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for pizza operations with vector search support
/// </summary>
public class VectorPizzaRepository : IVectorPizzaRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VectorPizzaRepository> _logger;

    public VectorPizzaRepository(
        AppDbContext context,
        ILogger<VectorPizzaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Pizza>> GetAvailableAsync(CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Pizza>> GetWithoutEmbeddingsAsync(CancellationToken ct = default)
    {
        return await _context.Pizzas
            .Where(p => p.Embedding == null)
            .ToListAsync(ct);
    }

    public async Task UpdateEmbeddingAsync(Guid pizzaId, float[] embedding, CancellationToken ct = default)
    {
        var pizza = await _context.Pizzas.FindAsync(new object[] { pizzaId }, ct);
        if (pizza != null)
        {
            pizza.Embedding = embedding;
            pizza.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("Updated embedding for pizza: {PizzaId}", pizzaId);
        }
    }

    public async Task<IEnumerable<Pizza>> SemanticSearchAsync(
        float[] queryEmbedding,
        float threshold = 0.5f,
        int maxResults = 5,
        CancellationToken ct = default)
    {
        try
        {
            var embeddingStr = "[" + string.Join(",", queryEmbedding) + "]";

            return await _context.Pizzas
                .FromSqlInterpolated($@"
                    SELECT p.* FROM kernelmind.pizzas p
                    WHERE p.""IsAvailable"" = true
                      AND p.""Embedding"" IS NOT NULL
                      AND (p.""Embedding"" <=> {embeddingStr}::vector) < {(1.0f - threshold)}
                    ORDER BY p.""Embedding"" <=> {embeddingStr}::vector
                    LIMIT {maxResults}")
                .AsNoTracking()
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during semantic search");
            return await GetAvailableAsync(ct);
        }
    }

    public async Task<IEnumerable<Pizza>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(ct);
    }
}
