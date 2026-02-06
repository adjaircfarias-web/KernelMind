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
    private readonly ILogger<MenuController> _logger;

    public MenuController(IPizzaRepository pizzaRepository, ILogger<MenuController> logger)
    {
        _pizzaRepository = pizzaRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets the complete menu with all available pizzas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pizza>>> GetMenu(CancellationToken ct)
    {
        _logger.LogInformation("Getting full menu");
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        return Ok(pizzas);
    }

    /// <summary>
    /// Gets a specific pizza by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Pizza>> GetPizza(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting pizza: {PizzaId}", id);
        var pizza = await _pizzaRepository.GetByIdAsync(id, ct);
        
        if (pizza == null)
            return NotFound();
        
        return Ok(pizza);
    }

    /// <summary>
    /// Searches for pizzas by name
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Pizza>>> SearchPizzas(
        [FromQuery] string query, 
        CancellationToken ct)
    {
        _logger.LogInformation("Searching pizzas for: {Query}", query);
        var pizzas = await _pizzaRepository.SearchByNameAsync(query, ct);
        return Ok(pizzas);
    }
}
