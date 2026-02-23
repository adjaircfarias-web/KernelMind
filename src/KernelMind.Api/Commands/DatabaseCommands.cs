using KernelMind.Core.Services;
using KernelMind.Infrastructure;
using KernelMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KernelMind.Api.Commands;

/// <summary>
/// CLI commands for database operations
/// </summary>
public static class DatabaseCommands
{
    /// <summary>
    /// Runs database seed and vectorization
    /// </summary>
    public static async Task RunDatabaseCommandAsync(string[] args, IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        var context = services.GetRequiredService<AppDbContext>();
        var vectorizationService = services.GetRequiredService<VectorizationService>();

        if (args.Contains("--seed", StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("Running database seed...");
            await SeedData.SeedAsync(context, logger);
            logger.LogInformation("Seed completed!");
        }

        if (args.Contains("--vectorize", StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("Starting menu vectorization...");
            var result = await vectorizationService.IndexAllPizzasAsync();
            logger.LogInformation("Vectorization completed: {Message}", result.Message);
            logger.LogInformation("Processed: {Success} successful, {Failed} failed",
                result.SuccessfulCount, result.FailedCount);
        }

        if (args.Contains("--reindex", StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("Starting menu re-indexing...");
            var result = await vectorizationService.ReindexAllPizzasAsync();
            logger.LogInformation("Re-indexing completed: {Message}", result.Message);
        }

        if (args.Contains("--status", StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("Getting vectorization status...");
            var status = await vectorizationService.GetStatusReportAsync();
            logger.LogInformation("Status: {Completed}/{Total} pizzas vectorized ({Percentage:F1}%)",
                status.PizzasWithEmbeddings, status.TotalPizzas, status.CompletionPercentage);
        }
    }
}
