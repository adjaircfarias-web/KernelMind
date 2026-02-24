using KernelMind.Api.Filters;
using KernelMind.Api.Middleware;
using KernelMind.Core;
using KernelMind.Core.Configuration;
using KernelMind.Core.Plugins;
using KernelMind.Core.Services;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure;
using KernelMind.Infrastructure.Data;
using KernelMind.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevCors", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("AppCors", policy =>
            {
                var allowedOrigins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() 
                    ?? new[]
                    {
                        "http://localhost:4200",
                        "http://localhost",
                        "http://127.0.0.1"
                    };

                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            options.UseNpgsql(dataSource);
        });

        builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
        builder.Services.AddScoped<IVectorPizzaRepository, VectorPizzaRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

        builder.Services.AddScoped<VectorSearchService>();
        builder.Services.AddScoped<VectorizationService>();
        builder.Services.AddScoped<EmbeddingService>();

        // Note: Plugins are registered as Singleton in AddKernelMindServices()
        builder.Services.AddKernelMindServices();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseCors("DevCors");
        }
        else
        {
            app.UseCors("AppCors");
        }

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseExceptionHandling();

        app.UseHttpsRedirection();

        app.MapControllers();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        var hasPizzas = await dbContext.Pizzas.AnyAsync();
        if (!hasPizzas)
        {
            logger.LogInformation("No pizzas found. Running automatic seed...");
            await SeedData.SeedAsync(dbContext, logger);
            logger.LogInformation("Auto-seed completed!");
        }

        app.Run();
    }
}
