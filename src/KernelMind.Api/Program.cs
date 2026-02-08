using KernelMind.Api.Filters;
using KernelMind.Api.Middleware;
using KernelMind.Core;
using KernelMind.Core.Plugins;
using KernelMind.Core.Services;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure;
using KernelMind.Infrastructure.Data;
using KernelMind.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KernelMind.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var seedOption = args.Contains("--seed", StringComparer.OrdinalIgnoreCase);

        builder.Services.AddOpenApi();

        builder.Services.AddControllers(options =>
        {
            options.AddValidationFilters();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
        builder.Services.AddScoped<IVectorPizzaRepository, VectorPizzaRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

        builder.Services.AddScoped<VectorSearchService>();
        builder.Services.AddScoped<VectorizationService>();
        builder.Services.AddScoped<EmbeddingService>();

        builder.Services.AddScoped<MenuPlugin>();
        builder.Services.AddScoped<OrderPlugin>();
        builder.Services.AddScoped<CalculationPlugin>();
        builder.Services.AddScoped<ContextPlugin>();

        builder.Services.AddKernelMindServices();

        var app = builder.Build();

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseExceptionHandling();

        app.UseHttpsRedirection();

        app.MapControllers();

        if (seedOption)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            logger.LogInformation("Running database seed...");
            await SeedData.SeedAsync(context, logger);
            logger.LogInformation("Seed completed!");
            
            return;
        }

        app.Run();
    }
}
