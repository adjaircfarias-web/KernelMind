using KernelMind.Core.Services;
using KernelMind.Domain.Entities;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KernelMind.Infrastructure;

/// <summary>
/// Seed data service for populating the database with initial pizza menu and embeddings
/// </summary>
public static class SeedData
{
    private static readonly PizzaData[] Pizzas = new[]
    {
        new PizzaData("Margherita", "A clássica pizza italiana com tomate, mussarela de búfala e manjericão fresco", 38.00m, "Tradicional", new[] { "Tomate", "Mussarela", "Manjericão", "Azeite" }),
        new PizzaData("Calabresa", "Pizza tradicional com calabresa defumada, cebola e azeitonas", 35.00m, "Tradicional", new[] { "Calabresa", "Cebola", "Azeitonas", "Orégano" }),
        new PizzaData("Portuguesa", "Pizza com ovos, presunto, mussarela, cebola e azeitonas", 42.00m, "Tradicional", new[] { "Ovo", "Presunto", "Mussarela", "Cebola", "Azeitonas" }),
        new PizzaData("Mussarela", "Simples e deliciosa pizza com generosa camada de mussarela", 32.00m, "Tradicional", new[] { "Mussarela", "Tomate", "Orégano" }),
        new PizzaData("Napolitana", "Pizza ao estilo napolitano com anchovas, alcaparras e azeitonas", 45.00m, "Tradicional", new[] { "Anchovas", "Alcaparras", "Azeitonas", "Alho", "Azeite" }),
        new PizzaData("Pepperoni", "Pizza americana com fatias crocantes de pepperoni e mussarela", 48.00m, "Especial", new[] { "Pepperoni", "Mussarela", "Orégano" }),
        new PizzaData("Quatro Queijos", "Combinação harmoniosa de quatro queijos Premium", 55.00m, "Especial", new[] { "Mussarela", "Provolone", "Parmesão", "Gorgonzola" }),
        new PizzaData("Frango com Catupiry", "Pizza cremosa com frango desfiado e catupiry original", 46.00m, "Especial", new[] { "Frango", "Catupiry", "Cebola", "Orégano" }),
        new PizzaData("Bacon Especial", "Pizza com bacon crocante, mussarela e cebolas caramelizadas", 50.00m, "Especial", new[] { "Bacon", "Mussarela", "Cebola", "Cheddar" }),
        new PizzaData("Supreme", "Pizza completa com pepperoni, cogumelos, pimentões e cebola", 58.00m, "Especial", new[] { "Pepperoni", "Cogumelos", "Pimentão", "Cebola", "Mussarela" }),
        new PizzaData("Mexicana", "Pizza apimentada com carne moída, jalapeño e feijão", 52.00m, "Especial", new[] { "Carne Moída", "Jalapeño", "Feijão", "Cheddar", "Cebola" }),
        new PizzaData("Chocolate", "Pizza doce com chocolate ao leite e granulado", 40.00m, "Doce", new[] { "Chocolate ao Leite", "Granulado", "Leite Condensado" }),
        new PizzaData("Prestígio", "Pizza doce com chocolate branco e coco ralado", 42.00m, "Doce", new[] { "Chocolate Branco", "Coco Ralado", "Leite Condensado" }),
        new PizzaData("Romeu e Julieta", "Pizza doce com goiabada cremosa e mussarela de primeira", 38.00m, "Doce", new[] { "Goiabada", "Mussarela", "Canela" }),
        new PizzaData("Banana com Canela", "Pizza doce com bananas fritas, açúcar e canela", 36.00m, "Doce", new[] { "Banana", "Açúcar", "Canela", "Manteiga" }),
        new PizzaData("Nutella", "Pizza generosa com Nutella e morangos frescos", 55.00m, "Doce", new[] { "Nutella", "Morango", "Farinha de Amêndoas" }),
    };

    private record PizzaData(string Name, string Description, decimal Price, string Category, string[] Ingredients);

    /// <summary>
    /// Seeds the database with initial pizza menu data and generates embeddings
    /// </summary>
    public static async Task SeedAsync(AppDbContext context, EmbeddingService embeddingService, ILogger logger, CancellationToken ct = default)
    {
        logger.LogInformation("Starting database seeding...");

        try
        {
            var existingCount = await context.Pizzas.CountAsync(ct);
            if (existingCount > 0)
            {
                logger.LogInformation("Database already contains {Count} pizzas. Skipping seed.", existingCount);
                return;
            }

            logger.LogInformation("Seeding {Count} pizzas with semantic embeddings...", Pizzas.Length);

            foreach (var pizzaData in Pizzas)
            {
                logger.LogInformation("Generating embedding for: {PizzaName}", pizzaData.Name);

                var embedding = await embeddingService.GeneratePizzaEmbeddingAsync(
                    pizzaData.Name,
                    pizzaData.Description,
                    pizzaData.Ingredients,
                    ct);

                var pizza = new Pizza
                {
                    Name = pizzaData.Name,
                    Description = pizzaData.Description,
                    Price = pizzaData.Price,
                    Category = pizzaData.Category,
                    Ingredients = pizzaData.Ingredients.ToList(),
                    IsAvailable = true,
                    Embedding = embedding
                };

                await context.Pizzas.AddAsync(pizza, ct);
            }

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Successfully seeded {Count} pizzas with embeddings!", Pizzas.Length);

            var byCategory = Pizzas.GroupBy(p => p.Category);
            foreach (var category in byCategory)
            {
                logger.LogInformation("  - {Category}: {Count} pizzas",
                    category.Key, category.Count());
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    /// <summary>
    /// Seeds without embeddings (fallback)
    /// </summary>
    public static async Task SeedAsync(AppDbContext context, ILogger logger, CancellationToken ct = default)
    {
        logger.LogInformation("Starting database seeding (without embeddings)...");

        try
        {
            var existingCount = await context.Pizzas.CountAsync(ct);
            if (existingCount > 0)
            {
                logger.LogInformation("Database already contains {Count} pizzas. Skipping seed.", existingCount);
                return;
            }

            logger.LogInformation("Seeding {Count} pizzas...", Pizzas.Length);

            foreach (var pizzaData in Pizzas)
            {
                var pizza = new Pizza
                {
                    Name = pizzaData.Name,
                    Description = pizzaData.Description,
                    Price = pizzaData.Price,
                    Category = pizzaData.Category,
                    Ingredients = pizzaData.Ingredients.ToList(),
                    IsAvailable = true,
                    Embedding = new float[768]
                };

                await context.Pizzas.AddAsync(pizza, ct);
            }

            await context.SaveChangesAsync(ct);
            logger.LogInformation("Successfully seeded {Count} pizzas!", Pizzas.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    /// <summary>
    /// Gets seed data summary for display
    /// </summary>
    public static string GetSummary()
    {
        var byCategory = Pizzas.GroupBy(p => p.Category);
        var summary = $"Seeding {Pizzas.Length} pizzas:\n";
        foreach (var category in byCategory)
        {
            summary += $"  - {category.Key}: {category.Count()} pizzas\n";
        }
        return summary;
    }
}
