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
    private readonly EmbeddingService _embeddingService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(
        IPizzaRepository pizzaRepository,
        EmbeddingService embeddingService,
        ILogger<MenuController> logger)
    {
        _pizzaRepository = pizzaRepository;
        _embeddingService = embeddingService;
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
            var embedding = await _embeddingService.GenerateEmbeddingAsync(query, ct);
            var pizzas = await _pizzaRepository.SemanticSearchAsync(query, embedding, threshold, maxResults, ct);
            
            _logger.LogInformation("Found {Count} pizzas matching query", pizzas.Count());
            return Ok(pizzas.Select(p => PizzaDto.FromEntity(p)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in semantic search");
            return StatusCode(500, new { error = "Search failed" });
        }
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
