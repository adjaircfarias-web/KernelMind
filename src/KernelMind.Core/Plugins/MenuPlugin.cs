using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for menu operations
/// </summary>
public class MenuPlugin
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly ILogger<MenuPlugin> _logger;

    public MenuPlugin(IPizzaRepository pizzaRepository, ILogger<MenuPlugin> logger)
    {
        _pizzaRepository = pizzaRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the complete pizza menu with all available items
    /// </summary>
    public async Task<string> GetMenuAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Getting full menu");
        
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        
        if (!pizzas.Any())
            return "Desculpe, não temos pizzas disponíveis no momento.";

        var menu = pizzas.Select(p => $"- {p.Name}: {p.Price:C} - {p.Description}");
        return "Nosso cardápio:\n\n" + string.Join("\n", menu);
    }

    /// <summary>
    /// Gets detailed information about a specific pizza by name
    /// </summary>
    public async Task<string> GetPizzaDetailsAsync(string pizzaName, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting details for pizza: {PizzaName}", pizzaName);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(pizzaName, ct);
        var pizza = pizzas.FirstOrDefault();
        
        if (pizza == null)
            return $"Não encontrei a pizza '{pizzaName}'. Use o menu para ver as pizzas disponíveis.";

        return $"**{pizza.Name}** - {pizza.Price:C}\n\n" +
               $"{pizza.Description}\n\n" +
               $"Ingredientes: {string.Join(", ", pizza.Ingredients)}\n" +
               $"Categoria: {pizza.Category}";
    }

    /// <summary>
    /// Searches for pizzas by ingredients or description
    /// </summary>
    public async Task<string> SearchPizzasAsync(string query, CancellationToken ct = default)
    {
        _logger.LogInformation("Searching pizzas for: {Query}", query);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(query, ct);
        
        if (!pizzas.Any())
            return $"Não encontrei pizzas relacionadas a '{query}'. Tente outra busca.";

        var results = pizzas.Select(p => $"- {p.Name}: {p.Price:C}");
        return $"Pizzas encontradas para '{query}':\n\n" + string.Join("\n", results);
    }
}
