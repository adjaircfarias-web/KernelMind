using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;

namespace KernelMind.Core.Services;

/// <summary>
/// Service for generating embeddings and performing vector operations
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
    /// Generates embeddings for multiple texts
    /// </summary>
    public async Task<IEnumerable<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, 
        CancellationToken ct = default)
    {
        var textList = texts.ToList();
        _logger.LogInformation("Generating embeddings for {Count} texts", textList.Count);
        
        var embeddings = new List<float[]>();
        
        foreach (var text in textList)
        {
            var embedding = await GenerateEmbeddingAsync(text, ct);
            embeddings.Add(embedding);
        }
        
        return embeddings;
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
}
