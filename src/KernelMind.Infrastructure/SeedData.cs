using KernelMind.Domain.Entities;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KernelMind.Infrastructure;

/// <summary>
/// Seed data service for populating the database with initial pizza menu
/// </summary>
public static class SeedData
{
    private static readonly Pizza[] Pizzas = new[]
    {
        // === TRADICIONAIS ===
        new Pizza
        {
            Name = "Margherita",
            Description = "A clássica pizza italiana com tomate, mussarela de búfala e manjericão fresco",
            Price = 38.00m,
            Category = "Tradicional",
            Ingredients = new List<string> { "Tomate", "Mussarela", "Manjericão", "Azeite" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Calabresa",
            Description = "Pizza tradicional com calabresa defumada, cebola e azeitonas",
            Price = 35.00m,
            Category = "Tradicional",
            Ingredients = new List<string> { "Calabresa", "Cebola", "Azeitonas", "Orégano" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Portuguesa",
            Description = "Pizza com ovos, presunto, mussarela, cebola e azeitonas",
            Price = 42.00m,
            Category = "Tradicional",
            Ingredients = new List<string> { "Ovo", "Presunto", "Mussarela", "Cebola", "Azeitonas" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Mussarela",
            Description = "Simples e deliciosa pizza com generosa camada de mussarela",
            Price = 32.00m,
            Category = "Tradicional",
            Ingredients = new List<string> { "Mussarela", "Tomate", "Orégano" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Napolitana",
            Description = "Pizza ao estilo napolitano com anchovas, alcaparras e azeitonas",
            Price = 45.00m,
            Category = "Tradicional",
            Ingredients = new List<string> { "Anchovas", "Alcaparras", "Azeitonas", "Alho", "Azeite" },
            IsAvailable = true
        },

        // === ESPECIAIS ===
        new Pizza
        {
            Name = "Pepperoni",
            Description = "Pizza americana com fatias crocantes de pepperoni e mussarela",
            Price = 48.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Pepperoni", "Mussarela", "Orégano" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Quatro Queijos",
            Description = "Combinação harmoniosa de quatro queijos Premium",
            Price = 55.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Mussarela", "Provolone", "Parmesão", "Gorgonzola" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Frango com Catupiry",
            Description = "Pizza cremosa com frango desfiado e catupiry original",
            Price = 46.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Frango", "Catupiry", "Cebola", "Orégano" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Bacon Especial",
            Description = "Pizza com bacon crocante, mussarela e cebolas caramelizadas",
            Price = 50.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Bacon", "Mussarela", "Cebola", "Cheddar" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Supreme",
            Description = "Pizza completa com pepperoni, cogumelos, pimentões e cebola",
            Price = 58.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Pepperoni", "Cogumelos", "Pimentão", "Cebola", "Mussarela" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Mexicana",
            Description = "Pizza apimentada com carne moída, jalapeño e feijão",
            Price = 52.00m,
            Category = "Especial",
            Ingredients = new List<string> { "Carne Moída", "Jalapeño", "Feijão", "Cheddar", "Cebola" },
            IsAvailable = true
        },

        // === DOCES ===
        new Pizza
        {
            Name = "Chocolate",
            Description = "Pizza doce com chocolate ao leite e granulado",
            Price = 40.00m,
            Category = "Doce",
            Ingredients = new List<string> { "Chocolate ao Leite", "Granulado", "Leite Condensado" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Prestígio",
            Description = "Pizza doce com chocolate branco e coco ralado",
            Price = 42.00m,
            Category = "Doce",
            Ingredients = new List<string> { "Chocolate Branco", "Coco Ralado", "Leite Condensado" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Romeu e Julieta",
            Description = "Pizza doce com goiabada cremosa e mussarela de primeira",
            Price = 38.00m,
            Category = "Doce",
            Ingredients = new List<string> { "Goiabada", "Mussarela", "Canela" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Banana com Canela",
            Description = "Pizza doce com bananas fritas, açúcar e canela",
            Price = 36.00m,
            Category = "Doce",
            Ingredients = new List<string> { "Banana", "Açúcar", "Canela", "Manteiga" },
            IsAvailable = true
        },
        new Pizza
        {
            Name = "Nutella",
            Description = "Pizza generosa com Nutella e morangos frescos",
            Price = 55.00m,
            Category = "Doce",
            Ingredients = new List<string> { "Nutella", "Morango", "Farinha de Amêndoas" },
            IsAvailable = true
        }
    };

    /// <summary>
    /// Seeds the database with initial pizza menu data
    /// </summary>
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Starting database seeding...");

        try
        {
            // Check if data already exists
            var existingCount = await context.Pizzas.CountAsync();
            if (existingCount > 0)
            {
                logger.LogInformation("Database already contains {Count} pizzas. Skipping seed.", existingCount);
                return;
            }

            logger.LogInformation("Seeding {Count} pizzas...", Pizzas.Length);

            foreach (var pizza in Pizzas)
            {
                await context.Pizzas.AddAsync(pizza);
            }

            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded {Count} pizzas!", Pizzas.Length);

            // Log summary by category
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
