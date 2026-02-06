using KernelMind.Core.Plugins;
using KernelMind.Core.Services;
using KernelMind.Domain.Interfaces;
using KernelMind.Infrastructure.Data;
using KernelMind.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace KernelMind.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        // Add Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "KernelMind API",
                Version = "v1",
                Description = "AI-powered Pizza Ordering Chatbot API"
            });
        });

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Configure Entity Framework
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=kernelmind;Username=postgres;Password=postgres123";
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.UseVector())
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

        // Register repositories
        builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

        // Configure Ollama AI Services
        ConfigureOllamaServices(builder);

        // Register services
        builder.Services.AddScoped<ChatService>();
        builder.Services.AddScoped<EmbeddingService>();

        // Register plugins
        builder.Services.AddScoped<MenuPlugin>();
        builder.Services.AddScoped<OrderPlugin>();
        builder.Services.AddScoped<CalculationPlugin>();
        builder.Services.AddScoped<ContextPlugin>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KernelMind API v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
        
        // Ollama health check
        app.MapGet("/health/ollama", async (IChatClient chatClient) =>
        {
            try
            {
                // Try to get a simple response to verify Ollama is working
                var response = await chatClient.CompleteAsync("Hi");
                
                return Results.Ok(new { status = "healthy", model = "ollama", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Ollama health check failed: {ex.Message}",
                    statusCode: 503);
            }
        });

        app.Run();
    }

    private static void ConfigureOllamaServices(WebApplicationBuilder builder)
    {
        var ollamaUrl = builder.Configuration["Ollama:Url"] ?? "http://localhost:11434";
        var ollamaModel = builder.Configuration["Ollama:Model"] ?? "llama3.1:8b";
        var embeddingModel = builder.Configuration["Ollama:EmbeddingModel"] ?? "nomic-embed-text";

        // Register Ollama Chat Client
        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Configuring Ollama chat client at {Url} with model {Model}", 
                ollamaUrl, ollamaModel);

            return new OllamaChatClient(new Uri(ollamaUrl), ollamaModel);
        });

        // Register Ollama Embedding Generator
        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Configuring Ollama embedding generator at {Url} with model {Model}", 
                ollamaUrl, embeddingModel);

            return new OllamaEmbeddingGenerator(new Uri(ollamaUrl), embeddingModel);
        });
    }
}
