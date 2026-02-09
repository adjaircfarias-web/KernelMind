using KernelMind.Core.Plugins;
using KernelMind.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace KernelMind.Core.Configuration;

/// <summary>
/// Extension methods for configuring Semantic Kernel with Ollama and Function Calling
/// </summary>
public static class KernelConfig
{
    /// <summary>
    /// Configures Semantic Kernel with Ollama and all plugins
    /// </summary>
    public static IServiceCollection AddSemanticKernel(
        this IServiceCollection services,
        Action<OllamaOptions>? configureOptions = null)
    {
        services.AddOptions<OllamaOptions>()
            .BindConfiguration(OllamaOptions.Ollama);
        
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add plugins as singletons for reuse
        services.AddSingleton<MenuPlugin>();
        services.AddSingleton<OrderPlugin>();
        services.AddSingleton<CalculationPlugin>();
        services.AddSingleton<ContextPlugin>();

        // Configure Semantic Kernel with Ollama
        services.AddKernel();
        
        services.AddSingleton<Kernel>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            
            var builder = Kernel.CreateBuilder();
            
            // Add Ollama chat completion with function calling support
            builder.AddOllamaChatCompletion(
                modelId: options.ChatModel,
                endpoint: new Uri(options.Host));
            
            // Add all plugins to the kernel
            builder.Plugins.AddFromObject(sp.GetRequiredService<MenuPlugin>(), "Menu");
            builder.Plugins.AddFromObject(sp.GetRequiredService<OrderPlugin>(), "Order");
            builder.Plugins.AddFromObject(sp.GetRequiredService<CalculationPlugin>(), "Calculation");
            builder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
            
            builder.Services.AddSingleton(loggerFactory);
            
            return builder.Build();
        });

        // Register IChatCompletionService
        services.AddSingleton<IChatCompletionService>(sp =>
        {
            var kernel = sp.GetRequiredService<Kernel>();
            return kernel.GetRequiredService<IChatCompletionService>();
        });

        return services;
    }

    /// <summary>
    /// Adds Ollama embedding generator to the DI container
    /// </summary>
    public static IServiceCollection AddOllamaEmbeddingGenerator(
        this IServiceCollection services)
    {
        services.AddSingleton<Microsoft.Extensions.AI.IEmbeddingGenerator<string, Microsoft.Extensions.AI.Embedding<float>>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return new Microsoft.Extensions.AI.OllamaEmbeddingGenerator(new Uri(options.Host), options.EmbeddingModel);
        });

        return services;
    }

    /// <summary>
    /// Adds all KernelMind services to the DI container
    /// </summary>
    public static IServiceCollection AddKernelMindServices(this IServiceCollection services)
    {
        services.AddSemanticKernel();
        services.AddOllamaEmbeddingGenerator();

        services.AddScoped<ChatService>();
        services.AddScoped<EmbeddingService>();

        return services;
    }
}
