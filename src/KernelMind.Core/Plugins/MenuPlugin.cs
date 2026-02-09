using KernelMind.Domain.Entities;
using KernelMind.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

/// <summary>
/// Plugin for menu operations with Semantic Kernel
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
    /// Lists all available pizzas on the menu.
    /// Use this when the customer wants to see the menu or asks what pizzas are available.
    /// </summary>
    [KernelFunction("get_menu")]
    [Description("Lists all available pizzas on the menu with prices and descriptions")]
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
    /// Gets detailed information about a specific pizza.
    /// Use this when the customer asks about a specific pizza by name.
    /// </summary>
    [KernelFunction("get_pizza_details")]
    [Description("Gets detailed information about a specific pizza including price, ingredients, and description")]
    public async Task<string> GetPizzaDetailsAsync(
        [Description("The exact name of the pizza (e.g., 'Calabresa', 'Margherita')")]
        string pizzaName, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting details for pizza: {PizzaName}", pizzaName);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(pizzaName, ct);
        var pizza = pizzas.FirstOrDefault();
        
        if (pizza == null)
            return $"Não encontrei a pizza '{pizzaName}' 🤔. Use *get_menu* para ver todas as pizzas disponíveis.";

        return $"🍕 **{pizza.Name}**\n\n" +
               $"💰 **Preço:** {pizza.Price:C}\n\n" +
               $"📝 **Descrição:**\n{pizza.Description}\n\n" +
               $"🧅 **Ingredientes:**\n{string.Join(", ", pizza.Ingredients)}\n\n" +
               $"🏷️ **Categoria:** {pizza.Category}\n" +
               $"{(pizza.IsAvailable ? "✅ Disponível" : "❌ Indisponível")}";
    }

    /// <summary>
    /// Searches for pizzas by ingredients or description.
    /// Use this when the customer is looking for pizzas with specific ingredients.
    /// </summary>
    [KernelFunction("search_pizzas")]
    [Description("Searches for pizzas by ingredients, description, or category")]
    public async Task<string> SearchPizzasAsync(
        [Description("The search query (ingredient, pizza name, or category)")]
        string query, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Searching pizzas for: {Query}", query);
        
        var pizzas = await _pizzaRepository.SearchByNameAsync(query, ct);
        
        if (!pizzas.Any())
            return $"Não encontrei pizzas relacionadas a '{query}' 😕\n\nTente buscar por outro ingrediente ou use *get_menu* para ver todas as pizzas.";

        var results = pizzas.Select(p => $"🍕 **{p.Name}** - {p.Price:C}");
        return $"🔍 **Resultados para '{query}'** ({pizzas.Count()} encontrados)\n\n" + 
               string.Join("\n\n", results) + 
               "\n\n💡 Use *get_pizza_details* com o nome da pizza para mais informações.";
    }
}
