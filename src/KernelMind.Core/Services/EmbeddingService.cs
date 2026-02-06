using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;

namespace KernelMind.Core.Services;

/// <summary>
/// Service for generating embeddings and performing vector operations with RAG support
/// </summary>
public class EmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        ILogger<EmbeddingService> logger)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Generates an embedding vector for the given text
    /// </summary>
    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        _logger.LogInformation("Generating embedding for text: {TextLength} characters", text.Length);
        
        try
        {
            var embeddings = await _embeddingGenerator.GenerateAsync(new[] { text }, cancellationToken: ct);
            return embeddings.First().Vector.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding");
            throw;
        }
    }

    /// <summary>
    /// Generates embedding for pizza search (combines name, description, and ingredients)
    /// </summary>
    public async Task<float[]> GeneratePizzaEmbeddingAsync(string name, string description, IEnumerable<string> ingredients, CancellationToken ct = default)
    {
        var combinedText = $"{name}. {description}. Ingredients: {string.Join(", ", ingredients)}";
        return await GenerateEmbeddingAsync(combinedText, ct);
    }

    /// <summary>
    /// Generates embeddings for multiple texts in batch
    /// </summary>
    public async Task<IEnumerable<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, 
        CancellationToken ct = default)
    {
        var textList = texts.ToList();
        _logger.LogInformation("Generating embeddings for {Count} texts", textList.Count);
        
        var results = new List<float[]>();
        
        foreach (var text in textList)
        {
            var embedding = await GenerateEmbeddingAsync(text, ct);
            results.Add(embedding);
        }
        
        return results;
    }

    /// <summary>
    /// Finds most similar text from a collection based on embedding similarity
    /// </summary>
    public async Task<(string Text, float Similarity, int Index)?> FindMostSimilarAsync(
        string query,
        IEnumerable<string> candidates,
        CancellationToken ct = default)
    {
        var candidateList = candidates.ToList();
        if (!candidateList.Any())
            return null;

        _logger.LogInformation("Finding most similar text among {Count} candidates", candidateList.Count);

        var queryEmbedding = await GenerateEmbeddingAsync(query, ct);
        var bestSimilarity = -1f;
        var bestIndex = 0;
        var bestText = candidateList[0];

        foreach (var candidate in candidateList.Select((text, index) => (text, index)))
        {
            var candidateEmbedding = await GenerateEmbeddingAsync(candidate.text, ct);
            var similarity = CalculateSimilarity(queryEmbedding, candidateEmbedding);
            
            if (similarity > bestSimilarity)
            {
                bestSimilarity = similarity;
                bestIndex = candidate.index;
                bestText = candidate.text;
            }
        }

        return (bestText, bestSimilarity, bestIndex);
    }

    /// <summary>
    /// RAG Search: Finds relevant pizzas based on semantic similarity
    /// </summary>
    public async Task<List<(string Text, float Similarity)>> FindRelevantPizzasAsync(
        string query,
        IEnumerable<(string Name, string Description, IEnumerable<string> Ingredients)> pizzas,
        float threshold = 0.5f,
        int maxResults = 5,
        CancellationToken ct = default)
    {
        var pizzaList = pizzas.ToList();
        if (!pizzaList.Any())
            return new List<(string, float)>();

        _logger.LogInformation("Searching for relevant pizzas: {Query}", query);

        var queryEmbedding = await GenerateEmbeddingAsync(query, ct);
        var results = new List<(string Name, float Similarity)>();

        foreach (var pizza in pizzaList)
        {
            var pizzaEmbedding = await GeneratePizzaEmbeddingAsync(
                pizza.Name, 
                pizza.Description, 
                pizza.Ingredients, 
                ct);
            
            var similarity = CalculateSimilarity(queryEmbedding, pizzaEmbedding);
            
            if (similarity >= threshold)
            {
                results.Add((pizza.Name, similarity));
            }
        }

        return results
            .OrderByDescending(r => r.Similarity)
            .Take(maxResults)
            .ToList();
    }

    /// <summary>
    /// Calculates cosine similarity between two vectors
    /// </summary>
    public float CalculateSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            throw new ArgumentException("Vectors must have the same dimension");

        float dotProduct = 0;
        float norm1 = 0;
        float norm2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            norm1 += vector1[i] * vector1[i];
            norm2 += vector2[i] * vector2[i];
        }

        return dotProduct / (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2));
    }

    /// <summary>
    /// Normalizes a vector to unit length
    /// </summary>
    public float[] Normalize(float[] vector)
    {
        var norm = (float)Math.Sqrt(vector.Sum(x => x * x));
        if (norm == 0)
            return vector;
        
        return vector.Select(x => x / norm).ToArray();
    }
}
