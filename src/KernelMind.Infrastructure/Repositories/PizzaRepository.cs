using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace KernelMind.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Pizza entity
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

    public async Task<IEnumerable<Pizza>> SearchByEmbeddingAsync(
        float[] embedding, 
        float threshold = 0.7f, 
        int maxResults = 10, 
        CancellationToken ct = default)
    {
        // For now, return all available pizzas
        // In production, this should use raw SQL with pgvector similarity search
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.IsAvailable)
            .Take(maxResults)
            .ToListAsync(ct);
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
