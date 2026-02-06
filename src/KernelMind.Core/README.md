# KernelMind.Core

## Purpose
Application layer containing business logic, Semantic Kernel plugins, and orchestration.

## Responsibilities
- **Semantic Kernel Plugins:**
  - MenuPlugin (menu query)
  - OrderPlugin (order management)
  - CalculationPlugin (price calculations)
  - ContextPlugin (conversation context)
- **Application Services:**
  - ChatService (chat orchestration)
  - EmbeddingService (RAG - embedding generation)
- **Application DTOs and Models**

## References
- KernelMind.Domain

## Expected Structure
```
KernelMind.Core/
├── Plugins/
│   ├── MenuPlugin.cs
│   ├── OrderPlugin.cs
│   ├── CalculationPlugin.cs
│   └── ContextPlugin.cs
├── Services/
│   ├── ChatService.cs
│   └── EmbeddingService.cs
└── README.md
```

## Useful Commands
```bash
# Create project
dotnet new classlib -n KernelMind.Core

# Add reference
dotnet add reference ../KernelMind.Domain

# Add NuGet packages
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.Ollama
```
