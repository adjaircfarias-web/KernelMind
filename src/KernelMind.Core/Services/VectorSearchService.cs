using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Services;

/// <summary>
/// Service for performing vector similarity searches using pgvector
/// </summary>
public class VectorSearchService
{
    private readonly IVectorPizzaRepository _pizzaRepository;
    private readonly EmbeddingService _embeddingService;
    private readonly ILogger<VectorSearchService> _logger;
    private const int DefaultTopK = 5;
    private const float DefaultThreshold = 0.5f;

    public VectorSearchService(
        IVectorPizzaRepository pizzaRepository,
        EmbeddingService embeddingService,
        ILogger<VectorSearchService> logger)
    {
        _pizzaRepository = pizzaRepository;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    /// <summary>
    /// Searches for pizzas using semantic similarity
    /// </summary>
    public async Task<List<PizzaSearchResult>> SemanticSearchAsync(
        string query,
        int topK = DefaultTopK,
        float threshold = DefaultThreshold,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Performing semantic search for: {Query}", query);

        try
        {
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, ct);

            var pizzas = await _pizzaRepository.SemanticSearchAsync(
                queryEmbedding,
                threshold,
                topK,
                ct);

            var results = pizzas
                .Select(p => new PizzaSearchResult
                {
                    Pizza = p,
                    Similarity = CalculateCosineSimilarity(queryEmbedding, p.Embedding!)
                })
                .OrderByDescending(r => r.Similarity)
                .ToList();

            _logger.LogInformation("Found {Count} relevant pizzas", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during semantic search");
            return new List<PizzaSearchResult>();
        }
    }

    /// <summary>
    /// Searches for pizzas combining text search and semantic similarity
    /// </summary>
    public async Task<List<PizzaSearchResult>> HybridSearchAsync(
        string query,
        int topK = DefaultTopK,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Performing hybrid search for: {Query}", query);

        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, ct);

        var pizzas = await _pizzaRepository.SemanticSearchAsync(
            queryEmbedding,
            0.3f,
            topK * 2,
            ct);

        var results = pizzas
            .Select(p => new PizzaSearchResult
            {
                Pizza = p,
                Similarity = CalculateCosineSimilarity(queryEmbedding, p.Embedding!)
            })
            .OrderByDescending(r => r.Similarity)
            .Take(topK)
            .ToList();

        return results;
    }

    /// <summary>
    /// Finds similar pizzas based on a reference pizza
    /// </summary>
    public async Task<List<PizzaSearchResult>> FindSimilarPizzasAsync(
        Guid pizzaId,
        int topK = 4,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Finding similar pizzas to: {PizzaId}", pizzaId);

        var allPizzas = await _pizzaRepository.GetAvailableAsync(ct);
        var referencePizza = allPizzas.FirstOrDefault(p => p.Id == pizzaId);

        if (referencePizza?.Embedding == null)
        {
            _logger.LogWarning("Reference pizza not found or has no embedding");
            return new List<PizzaSearchResult>();
        }

        var otherPizzas = allPizzas.Where(p => p.Id != pizzaId && p.Embedding != null);

        var results = otherPizzas
            .Select(p => new PizzaSearchResult
            {
                Pizza = p,
                Similarity = CalculateCosineSimilarity(referencePizza.Embedding!, p.Embedding!)
            })
            .OrderByDescending(r => r.Similarity)
            .Take(topK)
            .ToList();

        return results;
    }

    /// <summary>
    /// Searches for pizzas by ingredients
    /// </summary>
    public async Task<List<PizzaSearchResult>> SearchByIngredientsAsync(
        IEnumerable<string> ingredients,
        int topK = DefaultTopK,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Searching pizzas by ingredients: {Ingredients}", string.Join(", ", ingredients));

        var query = string.Join(" ", ingredients);
        return await SemanticSearchAsync(query, topK, 0.3f, ct);
    }

    /// <summary>
    /// Gets trending pizzas based on recent updates
    /// </summary>
    public async Task<List<Pizza>> GetTrendingPizzasAsync(
        int count = 5,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting trending pizzas");

        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        return pizzas
            .OrderByDescending(p => p.UpdatedAt)
            .Take(count)
            .ToList();
    }

    private static float CalculateCosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            return 0;

        float dotProduct = 0;
        float norm1 = 0;
        float norm2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            norm1 += vector1[i] * vector1[i];
            norm2 += vector2[i] * vector2[i];
        }

        var denominator = (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2));
        return denominator == 0 ? 0 : dotProduct / denominator;
    }
}

/// <summary>
/// Result of a pizza search with similarity score
/// </summary>
public class PizzaSearchResult
{
    public required Pizza Pizza { get; init; }
    public float Similarity { get; init; }

    public string GetFormattedSimilarity() => $"{Similarity:P1}";

    public bool IsHighSimilarity() => Similarity >= 0.8f;
    public bool IsMediumSimilarity() => Similarity >= 0.5f && Similarity < 0.8f;
    public bool IsLowSimilarity() => Similarity < 0.5f;
}
