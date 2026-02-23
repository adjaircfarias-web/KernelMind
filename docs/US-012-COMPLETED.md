# US-012-COMPLETED: Configurar Semantic Kernel

**Date:** February 6, 2026  
**Status:** ✅ COMPLETED  
**Duration:** 3 hours

## Objective
Configure Semantic Kernel with Ollama local LLM integration for chat and embeddings.

## Completed Tasks

### 1. NuGet Packages Configuration
Added the following packages to `KernelMind.Core.csproj`:
- `Microsoft.Extensions.AI` - 9.0.1-preview.1.24570.5
- `Microsoft.Extensions.AI.Ollama` - 9.0.1-preview.1.24570.5
- `Microsoft.Extensions.Logging` - 9.0.1
- `Microsoft.Extensions.Logging.Abstractions` - 9.0.1
- `Microsoft.Extensions.Logging.Console` - 9.0.1
- `Microsoft.Extensions.Options` - 9.0.1
- `Microsoft.Extensions.Options.ConfigurationExtensions` - 9.0.1

### 2. Configuration Files

#### 2.1 appsettings.json (KernelMind.Api)
Added Ollama configuration section:
```json
{
  "Ollama": {
    "Host": "http://localhost:11434",
    "ChatModel": "llama3.1:70b",
    "EmbeddingModel": "nomic-embed-text"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=kernelmind;Username=kernelmind;Password=kernelmind"
  }
}
```

### 3. Configuration Classes

#### 3.1 OllamaOptions.cs (New)
```csharp
public class OllamaOptions
{
    public const string Ollama = "Ollama";
    public string Host { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3.1:70b";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
}
```

#### 3.2 KernelConfig.cs (New)
Extension methods for configuring Ollama services:
- `AddOllamaChatClient()` - Registers `IChatClient` with OllamaChatClient
- `AddOllamaEmbeddingGenerator()` - Registers `IEmbeddingGenerator<string, Embedding<float>>` with OllamaEmbeddingGenerator
- `AddKernelMindServices()` - Registers ChatService and EmbeddingService

### 4. Dependency Injection Setup (Program.cs)

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

builder.Services.AddScoped<MenuPlugin>();
builder.Services.AddScoped<OrderPlugin>();
builder.Services.AddScoped<CalculationPlugin>();
builder.Services.AddScoped<ContextPlugin>();

builder.Services.AddKernelMindServices();
```

### 5. Services Implementation

#### 5.1 IKernelService.cs (New)
Interface defining the contract for AI services:
```csharp
public interface IKernelService
{
    IChatClient ChatClient { get; }
    Task<string> ProcessMessageAsync(string sessionId, string message, CancellationToken ct = default);
    IAsyncEnumerable<string> StreamMessageAsync(string sessionId, string message, CancellationToken ct = default);
}
```

#### 5.2 ChatService.cs (Updated)
- Now implements `IKernelService`
- Added `ChatClient` property
- ProcessMessageAsync returns full response
- StreamMessageAsync yields chunks for streaming
- System prompt configured for pizza chatbot persona

#### 5.3 EmbeddingService.cs (Existing)
- `GenerateEmbeddingAsync(string text)` - Generates embedding vector
- `GenerateEmbeddingsAsync(IEnumerable<string> texts)` - Batch generation
- `CalculateSimilarity(float[] vector1, float[] vector2)` - Cosine similarity

### 6. API Controllers

#### 6.1 ChatController (Updated)
Endpoints:
- `POST /api/chat/message` - Synchronous chat response
- `POST /api/chat/stream` - Streaming chat response (IAsyncEnumerable)

Request/Response DTOs:
```csharp
public record ChatRequest(string Message, string? SessionId);
public record ChatResponse(string Content, string SessionId, DateTime Timestamp);
```

### 7. Plugins (Existing)

| Plugin | Purpose |
|--------|---------|
| MenuPlugin | Menu operations (list, search, details) |
| OrderPlugin | Order management (add, remove, confirm) |
| CalculationPlugin | Price calculations and discounts |
| ContextPlugin | Chat history management |

## Configuration Files Modified

| File | Change |
|------|--------|
| `src/KernelMind.Api/appsettings.json` | Added Ollama and ConnectionStrings sections |
| `src/KernelMind.Api/Program.cs` | Added DI configuration for all services |
| `src/KernelMind.Core/KernelMind.Core.csproj` | Added required NuGet packages |
| `src/KernelMind.Core/Configuration/OllamaOptions.cs` | Created new file |
| `src/KernelMind.Core/Configuration/KernelConfig.cs` | Created new file |
| `src/KernelMind.Core/Services/IKernelService.cs` | Created new file |
| `src/KernelMind.Core/Services/ChatService.cs` | Updated to implement IKernelService |

## Ollama Configuration

| Setting | Value | Description |
|---------|-------|-------------|
| Host | http://localhost:11434 | Ollama API endpoint |
| ChatModel | llama3.1:70b | LLM for chat completions |
| EmbeddingModel | nomic-embed-text | Model for text embeddings |

## Next Steps

1. **Test Ollama Connection**: Verify Ollama is running and models are loaded
   ```bash
   curl http://localhost:11434/api/version
   ```

2. **Pull Required Models**:
   ```bash
   ollama pull llama3.1:70b
   ollama pull nomic-embed-text
   ```

3. **Run Health Check**:
   ```bash
   dotnet run --project src/KernelMind.Api
   # Test: GET http://localhost:5076/swagger
   ```

4. **Implement US-013**: MenuPlugin integration with Semantic Kernel functions

## Notes

- Using `Microsoft.Extensions.AI` abstraction layer for Ollama integration
- OllamaChatClient and OllamaEmbeddingGenerator from `Microsoft.Extensions.AI` namespace
- Streaming implemented with `IAsyncEnumerable<string>` and HTTP streaming
- System prompt configured for Portuguese pizza chatbot persona
- All services registered as scoped (per-request lifetime)

---
**Completed by:** AI Assistant  
**Review required:** Yes
