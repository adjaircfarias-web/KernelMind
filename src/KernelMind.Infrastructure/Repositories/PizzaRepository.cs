using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Pizza entity with RAG support
/// </summary>
public class PizzaRepository : IPizzaRepository
{
    private readonly AppDbContext _context;

    public PizzaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pizza>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Pizza>> GetAvailableAsync(CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<Pizza?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Pizza>> SearchByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.IsAvailable && EF.Functions.Like(p.Name, $"%{name}%"))
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Searches for similar pizzas using vector similarity search
    /// Uses raw SQL for pgvector similarity calculation
    /// </summary>
    public async Task<IEnumerable<Pizza>> SearchByEmbeddingAsync(
        float[] embedding, 
        float threshold = 0.7f, 
        int maxResults = 10, 
        CancellationToken ct = default)
    {
        try
        {
            var embeddingStr = "[" + string.Join(",", embedding) + "]";
            
            var pizzas = await _context.Pizzas
                .FromSqlRaw(
                    @"SELECT p.*, 1 as Discriminator
                      FROM kernelmind.pizzas p
                      WHERE p.""IsAvailable"" = true
                        AND p.""Embedding"" IS NOT NULL
                      ORDER BY p.""Embedding"" <=> '{0}'::vector
                      LIMIT {1}",
                    embeddingStr,
                    maxResults)
                .AsNoTracking()
                .ToListAsync(ct);
                
            return pizzas;
        }
        catch (Exception ex)
        {
            _context.ChangeTracker.Clear();
            
            return await _context.Pizzas
                .AsNoTracking()
                .Where(p => p.IsAvailable)
                .Take(maxResults)
                .ToListAsync(ct);
        }
    }

    /// <summary>
    /// Performs semantic search combining text and embedding similarity
    /// </summary>
    public async Task<IEnumerable<Pizza>> SemanticSearchAsync(
        string query,
        float[] embedding,
        float threshold = 0.5f,
        int maxResults = 10,
        CancellationToken ct = default)
    {
        try
        {
            var embeddingStr = "[" + string.Join(",", embedding) + "]";
            
            var pizzas = await _context.Pizzas
                .FromSqlRaw(
                    @"SELECT p.*, 1 as Discriminator
                      FROM kernelmind.pizzas p
                      WHERE p.""IsAvailable"" = true
                        AND (p.""Embedding"" <=> '{0}'::vector) < {1}
                      ORDER BY p.""Embedding"" <=> '{0}'::vector
                      LIMIT {2}",
                    embeddingStr,
                    1.0f - threshold,
                    maxResults)
                .AsNoTracking()
                .ToListAsync(ct);
                
            return pizzas;
        }
        catch
        {
            _context.ChangeTracker.Clear();
            
            return await SearchByNameAsync(query, ct);
        }
    }

    public async Task<Pizza> CreateAsync(Pizza pizza, CancellationToken ct = default)
    {
        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync(ct);
        return pizza;
    }

    public async Task UpdateAsync(Pizza pizza, CancellationToken ct = default)
    {
        _context.Pizzas.Update(pizza);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var pizza = await _context.Pizzas.FindAsync(new object[] { id }, ct);
        if (pizza != null)
        {
            _context.Pizzas.Remove(pizza);
            await _context.SaveChangesAsync(ct);
        }
    }
}
