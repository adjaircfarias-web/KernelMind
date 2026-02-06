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
    /// Lists all available pizzas on the menu
    /// </summary>
    public async Task<string> GetMenuAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Getting full menu");
        
        var pizzas = await _pizzaRepository.GetAvailableAsync(ct);
        
        if (!pizzas.Any())
            return "Desculpe, não temos pizzas disponíveis no momento. 😔";

        var menu = pizzas.Select(p => $"🍕 **{p.Name}** - {p.Price:C}\n   {p.Description}");
        return $"🍕 **Nosso Cardápio**\n\n" + string.Join("\n\n", menu) + 
               $"\n\n💡 **{pizzas.Count()}** pizzas disponíveis";
    }

    /// <summary>
    /// Gets detailed information about a specific pizza
    /// </summary>
    public async Task<string> GetPizzaDetailsAsync(
        string pizzaName, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting details for pizza: {PizzaName}", pizzaName);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(pizzaName, ct);
        var pizza = pizzas.FirstOrDefault();
        
        if (pizza == null)
            return $"Não encontrei a pizza '{pizzaName}' 🤔. Use *list_menu* para ver todas as pizzas disponíveis.";

        return $"🍕 **{pizza.Name}**\n\n" +
               $"💰 **Preço:** {pizza.Price:C}\n\n" +
               $"📝 **Descrição:**\n{pizza.Description}\n\n" +
               $"🧅 **Ingredientes:**\n{string.Join(", ", pizza.Ingredients)}\n\n" +
               $"🏷️ **Categoria:** {pizza.Category}\n" +
               $"{(pizza.IsAvailable ? "✅ Disponível" : "❌ Indisponível")}";
    }

    /// <summary>
    /// Searches for pizzas by ingredients or description
    /// </summary>
    public async Task<string> SearchPizzasAsync(
        string query, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Searching pizzas for: {Query}", query);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(query, ct);
        
        if (!pizzas.Any())
            return $"Não encontrei pizzas relacionadas a '{query}' 😕\n\nTente buscar por outro ingrediente ou use *list_menu* para ver todas as pizzas.";

        var results = pizzas.Select(p => $"🍕 **{p.Name}** - {p.Price:C}");
        return $"🔍 **Resultados para '{query}'** ({pizzas.Count()} encontrados)\n\n" + 
               string.Join("\n\n", results) + 
               "\n\n💡 Use *get_pizza_details* com o nome da pizza para mais informações.";
    }
}
