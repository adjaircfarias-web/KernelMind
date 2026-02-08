using KernelMind.Core.Services;
using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KernelMind.Api.Controllers;

/// <summary>
/// API controller for menu operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IVectorPizzaRepository _vectorPizzaRepository;
    private readonly VectorSearchService _vectorSearchService;
    private readonly VectorizationService _vectorizationService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(
        IPizzaRepository pizzaRepository,
        IVectorPizzaRepository vectorPizzaRepository,
        VectorSearchService vectorSearchService,
        VectorizationService vectorizationService,
        ILogger<MenuController> logger)
    {
        _pizzaRepository = pizzaRepository;
        _vectorPizzaRepository = vectorPizzaRepository;
        _vectorSearchService = vectorSearchService;
        _vectorizationService = vectorizationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the complete menu with all available pizzas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> GetMenu(CancellationToken ct)
    {
        _logger.LogInformation("Getting full menu");
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        return Ok(pizzas.Select(p => PizzaDto.FromEntity(p)));
    }

    /// <summary>
    /// Gets a specific pizza by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PizzaDto>> GetPizza(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting pizza: {PizzaId}", id);
        var pizza = await _pizzaRepository.GetByIdAsync(id, ct);
        
        if (pizza == null)
            return NotFound(new { error = "Pizza not found" });
        
        return Ok(PizzaDto.FromEntity(pizza));
    }

    /// <summary>
    /// Searches for pizzas by name (text search)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> SearchPizzas(
        [FromQuery] string query, 
        CancellationToken ct)
    {
        _logger.LogInformation("Searching pizzas for: {Query}", query);
        var pizzas = await _pizzaRepository.SearchByNameAsync(query, ct);
        return Ok(pizzas.Select(p => PizzaDto.FromEntity(p)));
    }

    /// <summary>
    /// Semantic search using vector embeddings (RAG)
    /// </summary>
    [HttpGet("semantic-search")]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> SemanticSearch(
        [FromQuery] string query,
        [FromQuery] float threshold = 0.5f,
        [FromQuery] int maxResults = 5,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Semantic search for: {Query}", query);
        
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { error = "Query is required" });

        try
        {
            var results = await _vectorSearchService.SemanticSearchAsync(query, maxResults, threshold, ct);
            return Ok(results.Select(r => PizzaDto.FromEntity(r.Pizza)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in semantic search");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Hybrid search combining text and semantic similarity
    /// </summary>
    [HttpGet("hybrid-search")]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> HybridSearch(
        [FromQuery] string query,
        [FromQuery] int maxResults = 5,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Hybrid search for: {Query}", query);
        
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { error = "Query is required" });

        try
        {
            var results = await _vectorSearchService.HybridSearchAsync(query, maxResults, ct);
            return Ok(results.Select(r => PizzaDto.FromEntity(r.Pizza)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in hybrid search");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Gets similar pizzas based on a reference pizza
    /// </summary>
    [HttpGet("{id:guid}/similar")]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> GetSimilarPizzas(
        Guid id,
        [FromQuery] int maxResults = 4,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Finding similar pizzas to: {PizzaId}", id);

        try
        {
            var results = await _vectorSearchService.FindSimilarPizzasAsync(id, maxResults, ct);
            return Ok(results.Select(r => PizzaDto.FromEntity(r.Pizza)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar pizzas");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Vectorizes all pizzas in the menu (RAG setup)
    /// </summary>
    [HttpPost("vectorize")]
    public async Task<ActionResult<VectorizationResultDto>> VectorizeMenu(CancellationToken ct)
    {
        _logger.LogInformation("Starting menu vectorization");
        
        try
        {
            var result = await _vectorizationService.IndexAllPizzasAsync(ct);
            return Ok(VectorizationResultDto.FromResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error vectorizing menu");
            return StatusCode(500, new { error = "Vectorization failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Re-vectorizes all pizzas (updates existing embeddings)
    /// </summary>
    [HttpPost("reindex")]
    public async Task<ActionResult<VectorizationResultDto>> ReindexMenu(CancellationToken ct)
    {
        _logger.LogInformation("Starting menu re-indexing");
        
        try
        {
            var result = await _vectorizationService.ReindexAllPizzasAsync(ct);
            return Ok(VectorizationResultDto.FromResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reindexing menu");
            return StatusCode(500, new { error = "Reindexing failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets vectorization status
    /// </summary>
    [HttpGet("vectorization-status")]
    public async Task<ActionResult<VectorizationStatusReport>> GetVectorizationStatus(CancellationToken ct)
    {
        return await _vectorizationService.GetStatusReportAsync(ct);
    }

    /// <summary>
    /// Gets pizzas by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> GetByCategory(
        string category, 
        CancellationToken ct)
    {
        _logger.LogInformation("Getting pizzas by category: {Category}", category);
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        var filtered = pizzas.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return Ok(filtered.Select(p => PizzaDto.FromEntity(p)));
    }

    /// <summary>
    /// Gets menu categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories(CancellationToken ct)
    {
        _logger.LogInformation("Getting menu categories");
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        var categories = pizzas.Select(p => p.Category).Distinct().OrderBy(c => c);
        return Ok(categories);
    }
}

/// <summary>
/// DTO for Pizza entity
/// </summary>
public record PizzaDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    List<string> Ingredients,
    bool IsAvailable
)
{
    public static PizzaDto FromEntity(Pizza pizza) => new(
        pizza.Id,
        pizza.Name,
        pizza.Description,
        pizza.Price,
        pizza.Category ?? "",
        pizza.Ingredients ?? new List<string>(),
        pizza.IsAvailable
    );
}

/// <summary>
/// DTO for vectorization result
/// </summary>
public record VectorizationResultDto(
    DateTime StartedAt,
    DateTime CompletedAt,
    int TotalPizzasToProcess,
    int SuccessfulCount,
    int FailedCount,
    string Message,
    bool IsComplete,
    double SuccessRate
)
{
    public static VectorizationResultDto FromResult(VectorizationResult result) => new(
        result.StartedAt,
        result.CompletedAt,
        result.TotalPizzasToProcess,
        result.SuccessfulCount,
        result.FailedCount,
        result.Message,
        result.IsComplete,
        result.SuccessRate
    );
}
