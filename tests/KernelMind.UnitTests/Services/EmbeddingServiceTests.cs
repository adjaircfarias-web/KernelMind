using FluentAssertions;
using KernelMind.Core.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace KernelMind.UnitTests.Services;

public class EmbeddingServiceTests
{
    private readonly Mock<IEmbeddingGenerator<string, Embedding<float>>> _embeddingGeneratorMock;
    private readonly Mock<ILogger<EmbeddingService>> _loggerMock;
    private readonly EmbeddingService _embeddingService;

    public EmbeddingServiceTests()
    {
        _embeddingGeneratorMock = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();
        _loggerMock = new Mock<ILogger<EmbeddingService>>();
        _embeddingService = new EmbeddingService(_embeddingGeneratorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void CalculateSimilarity_WithValidEmbeddings_ReturnsSimilarity()
    {
        var embedding1 = Enumerable.Repeat(0.5f, 768).ToArray();
        var embedding2 = Enumerable.Repeat(0.5f, 768).ToArray();

        var result = _embeddingService.CalculateSimilarity(embedding1, embedding2);

        result.Should().BeGreaterOrEqualTo(0);
        result.Should().BeLessOrEqualTo(1);
    }

    [Fact]
    public void CalculateSimilarity_WithIdenticalEmbeddings_ReturnsOne()
    {
        var embedding = Enumerable.Repeat(1f, 768).ToArray();

        var result = _embeddingService.CalculateSimilarity(embedding, embedding);

        result.Should().BeApproximately(1.0f, 0.001f);
    }

    [Fact]
    public void CalculateSimilarity_WithDifferentEmbeddings_ReturnsLessThanOne()
    {
        var embedding1 = Enumerable.Range(0, 768).Select(i => (float)i).ToArray();
        var embedding2 = Enumerable.Range(768, 768).Select(i => (float)i).ToArray();

        var result = _embeddingService.CalculateSimilarity(embedding1, embedding2);

        result.Should().BeLessThan(1f);
    }

    [Fact]
    public void CalculateSimilarity_WithDifferentDimensions_ThrowsException()
    {
        var embedding1 = new float[768];
        var embedding2 = new float[384];

        Action act = () => _embeddingService.CalculateSimilarity(embedding1, embedding2);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Normalize_WithZeroVector_ReturnsZeroVector()
    {
        var vector = new float[768];

        var result = _embeddingService.Normalize(vector);

        result.Should().AllBeEquivalentTo(0f);
    }

    [Fact]
    public void Normalize_WithNonZeroVector_ReturnsNormalizedVector()
    {
        var vector = new float[768];
        vector[0] = 10f;
        vector[1] = 10f;

        var result = _embeddingService.Normalize(vector);

        result.Should().HaveCount(768);
        var magnitude = (float)Math.Sqrt(result.Take(2).Sum(x => x * x));
        magnitude.Should().BeApproximately(1f, 0.001f);
    }
}
