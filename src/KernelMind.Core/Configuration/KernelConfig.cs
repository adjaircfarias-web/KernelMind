using KernelMind.Core.Configuration;
using KernelMind.Core.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KernelMind.Core;

/// <summary>
/// Extension methods for configuring Semantic Kernel with Ollama
/// </summary>
public static class KernelConfig
{
    /// <summary>
    /// Adds Ollama chat and embedding services to the DI container
    /// </summary>
    public static IServiceCollection AddOllamaChatClient(
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

        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return new OllamaChatClient(new Uri(options.Host), options.ChatModel);
        });

        return services;
    }

    /// <summary>
    /// Adds Ollama embedding generator to the DI container
    /// </summary>
    public static IServiceCollection AddOllamaEmbeddingGenerator(
        this IServiceCollection services)
    {
        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return new OllamaEmbeddingGenerator(new Uri(options.Host), options.EmbeddingModel);
        });

        return services;
    }

    /// <summary>
    /// Adds all KernelMind services to the DI container
    /// </summary>
    public static IServiceCollection AddKernelMindServices(this IServiceCollection services)
    {
        services.AddOllamaChatClient();
        services.AddOllamaEmbeddingGenerator();

        services.AddScoped<ChatService>();
        services.AddScoped<EmbeddingService>();

        return services;
    }
}
