using KernelMind.Domain.Entities;

namespace KernelMind.Domain.Interfaces;

/// <summary>
/// Repository interface for pizza operations with vector search support
/// </summary>
public interface IVectorPizzaRepository
{
    /// <summary>
    /// Gets all available pizzas
    /// </summary>
    Task<IEnumerable<Pizza>> GetAvailableAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets all pizzas without embeddings
    /// </summary>
    Task<IEnumerable<Pizza>> GetWithoutEmbeddingsAsync(CancellationToken ct = default);

    /// <summary>
    /// Updates the embedding for a pizza
    /// </summary>
    Task UpdateEmbeddingAsync(Guid pizzaId, float[] embedding, CancellationToken ct = default);

    /// <summary>
    /// Searches for pizzas using semantic similarity
    /// </summary>
    Task<IEnumerable<Pizza>> SemanticSearchAsync(
        float[] queryEmbedding,
        float threshold = 0.5f,
        int maxResults = 5,
        CancellationToken ct = default);

    /// <summary>
    /// Gets pizzas by IDs
    /// </summary>
    Task<IEnumerable<Pizza>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
