using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;

namespace KernelMind.Core.Services;

/// <summary>
/// Service for indexing/vectorizing all pizzas in the menu
/// </summary>
public class VectorizationService
{
    private readonly IVectorPizzaRepository _pizzaRepository;
    private readonly EmbeddingService _embeddingService;
    private readonly ILogger<VectorizationService> _logger;

    public VectorizationService(
        IVectorPizzaRepository pizzaRepository,
        EmbeddingService embeddingService,
        ILogger<VectorizationService> logger)
    {
        _pizzaRepository = pizzaRepository;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    /// <summary>
    /// Vectorizes all pizzas that don't have embeddings yet
    /// </summary>
    public async Task<VectorizationResult> IndexAllPizzasAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting pizza vectorization process");

        var result = new VectorizationResult
        {
            StartedAt = DateTime.UtcNow
        };

        try
        {
            var pizzasWithoutEmbeddings = await _pizzaRepository.GetWithoutEmbeddingsAsync(ct);
            var pizzasList = pizzasWithoutEmbeddings.ToList();

            result.TotalPizzasToProcess = pizzasList.Count;

            if (pizzasList.Count == 0)
            {
                _logger.LogInformation("No pizzas to vectorize - all pizzas already have embeddings");
                result.Message = "All pizzas are already vectorized!";
                result.CompletedAt = DateTime.UtcNow;
                return result;
            }

            _logger.LogInformation("Found {Count} pizzas to vectorize", pizzasList.Count);

            int successCount = 0;
            int failureCount = 0;

            foreach (var pizza in pizzasList)
            {
                if (ct.IsCancellationRequested)
                {
                    _logger.LogWarning("Vectorization cancelled by user");
                    break;
                }

                try
                {
                    _logger.LogInformation("Vectorizing pizza: {PizzaName} ({PizzaId})", pizza.Name, pizza.Id);

                    var embedding = await _embeddingService.GeneratePizzaEmbeddingAsync(
                        pizza.Name,
                        pizza.Description,
                        pizza.Ingredients,
                        ct);

                    await _pizzaRepository.UpdateEmbeddingAsync(pizza.Id, embedding, ct);

                    successCount++;
                    result.ProcessedPizzas.Add(new PizzaVectorizationStatus
                    {
                        PizzaId = pizza.Id,
                        PizzaName = pizza.Name,
                        Status = VectorizationStatus.Success,
                        ProcessedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation("Successfully vectorized pizza: {PizzaName}", pizza.Name);
                }
                catch (Exception ex)
                {
                    failureCount++;
                    result.ProcessedPizzas.Add(new PizzaVectorizationStatus
                    {
                        PizzaId = pizza.Id,
                        PizzaName = pizza.Name,
                        Status = VectorizationStatus.Failed,
                        ErrorMessage = ex.Message,
                        ProcessedAt = DateTime.UtcNow
                    });

                    _logger.LogError(ex, "Failed to vectorize pizza: {PizzaName}", pizza.Name);
                }
            }

            result.SuccessfulCount = successCount;
            result.FailedCount = failureCount;
            result.Message = $"Processed {successCount + failureCount} pizzas: {successCount} successful, {failureCount} failed";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during vectorization process");
            result.Message = $"Error: {ex.Message}";
        }

        result.CompletedAt = DateTime.UtcNow;
        _logger.LogInformation("Vectorization completed: {Message}", result.Message);

        return result;
    }

    /// <summary>
    /// Re-vectorizes all pizzas (updates existing embeddings)
    /// </summary>
    public async Task<VectorizationResult> ReindexAllPizzasAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting pizza re-indexing process (will update all embeddings)");

        var result = new VectorizationResult
        {
            StartedAt = DateTime.UtcNow
        };

        try
        {
            var allPizzas = await _pizzaRepository.GetAvailableAsync(ct);
            var pizzasList = allPizzas.ToList();

            result.TotalPizzasToProcess = pizzasList.Count;

            if (pizzasList.Count == 0)
            {
                result.Message = "No pizzas found to reindex";
                result.CompletedAt = DateTime.UtcNow;
                return result;
            }

            _logger.LogInformation("Reindexing {Count} pizzas", pizzasList.Count);

            int successCount = 0;
            int failureCount = 0;

            foreach (var pizza in pizzasList)
            {
                if (ct.IsCancellationRequested)
                {
                    _logger.LogWarning("Reindexing cancelled by user");
                    break;
                }

                try
                {
                    var embedding = await _embeddingService.GeneratePizzaEmbeddingAsync(
                        pizza.Name,
                        pizza.Description,
                        pizza.Ingredients,
                        ct);

                    await _pizzaRepository.UpdateEmbeddingAsync(pizza.Id, embedding, ct);

                    successCount++;
                    result.ProcessedPizzas.Add(new PizzaVectorizationStatus
                    {
                        PizzaId = pizza.Id,
                        PizzaName = pizza.Name,
                        Status = VectorizationStatus.Success,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    failureCount++;
                    result.ProcessedPizzas.Add(new PizzaVectorizationStatus
                    {
                        PizzaId = pizza.Id,
                        PizzaName = pizza.Name,
                        Status = VectorizationStatus.Failed,
                        ErrorMessage = ex.Message,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
            }

            result.SuccessfulCount = successCount;
            result.FailedCount = failureCount;
            result.Message = $"Reindexed {successCount + failureCount} pizzas: {successCount} successful, {failureCount} failed";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during reindexing process");
            result.Message = $"Error: {ex.Message}";
        }

        result.CompletedAt = DateTime.UtcNow;
        return result;
    }

    /// <summary>
    /// Gets the vectorization status of all pizzas
    /// </summary>
    public async Task<VectorizationStatusReport> GetStatusReportAsync(CancellationToken ct = default)
    {
        var allPizzas = await _pizzaRepository.GetAvailableAsync(ct);
        var withoutEmbeddings = await _pizzaRepository.GetWithoutEmbeddingsAsync(ct);

        return new VectorizationStatusReport
        {
            TotalPizzas = allPizzas.Count(),
            PizzasWithEmbeddings = allPizzas.Count() - withoutEmbeddings.Count(),
            PizzasWithoutEmbeddings = withoutEmbeddings.Count(),
            CompletionPercentage = allPizzas.Any() 
                ? (double)(allPizzas.Count() - withoutEmbeddings.Count()) / allPizzas.Count() * 100 
                : 100
        };
    }
}

/// <summary>
/// Result of a vectorization operation
/// </summary>
public class VectorizationResult
{
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public int TotalPizzasToProcess { get; set; }
    public int SuccessfulCount { get; set; }
    public int FailedCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<PizzaVectorizationStatus> ProcessedPizzas { get; set; } = new();

    public bool IsComplete => TotalPizzasToProcess > 0 && (SuccessfulCount + FailedCount) >= TotalPizzasToProcess;
    public double SuccessRate => TotalPizzasToProcess > 0 ? (double)SuccessfulCount / TotalPizzasToProcess * 100 : 0;
}

/// <summary>
/// Status of a single pizza vectorization
/// </summary>
public class PizzaVectorizationStatus
{
    public Guid PizzaId { get; set; }
    public string PizzaName { get; set; } = string.Empty;
    public VectorizationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Status of vectorization for all pizzas
/// </summary>
public class VectorizationStatusReport
{
    public int TotalPizzas { get; set; }
    public int PizzasWithEmbeddings { get; set; }
    public int PizzasWithoutEmbeddings { get; set; }
    public double CompletionPercentage { get; set; }
    public bool IsFullyVectorized => PizzasWithoutEmbeddings == 0;
}

/// <summary>
/// Status of a vectorization operation
/// </summary>
public enum VectorizationStatus
{
    Pending,
    Processing,
    Success,
    Failed,
    Cancelled
}
