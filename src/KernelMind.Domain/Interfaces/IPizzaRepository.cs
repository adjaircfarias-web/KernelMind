using KernelMind.Domain.Entities;

namespace KernelMind.Domain.Interfaces;

/// <summary>
/// Repository interface for Pizza entity
/// </summary>
public interface IPizzaRepository
{
    Task<IEnumerable<Pizza>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Pizza>> GetAvailableAsync(CancellationToken ct = default);
    Task<Pizza?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Pizza>> SearchByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Pizza>> SearchByEmbeddingAsync(float[] embedding, float threshold = 0.7f, int maxResults = 10, CancellationToken ct = default);
    Task<IEnumerable<Pizza>> SemanticSearchAsync(string query, float[] embedding, float threshold = 0.5f, int maxResults = 5, CancellationToken ct = default);
    Task<Pizza> CreateAsync(Pizza pizza, CancellationToken ct = default);
    Task UpdateAsync(Pizza pizza, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
