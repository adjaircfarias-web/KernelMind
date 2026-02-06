# KernelMind.Infrastructure

## Purpose
Infrastructure layer containing concrete implementations for data access, external integrations, and infrastructure services.

## Responsibilities
- **Data Access:**
  - AppDbContext (Entity Framework Core)
  - Migrations
  - Entity configurations
- **Repositories:**
  - PizzaRepository
  - OrderRepository
  - ChatSessionRepository
- **External Integrations:**
  - OllamaClient (local LLM communication)
  - VectorStore (pgvector for RAG)
- **Infrastructure Services:**
  - LoggingService

## References
- KernelMind.Domain

## Expected Structure
```
KernelMind.Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   └── Configurations/
├── Repositories/
│   ├── PizzaRepository.cs
│   ├── OrderRepository.cs
│   └── ChatSessionRepository.cs
├── Services/
│   ├── OllamaClient.cs
│   └── LoggingService.cs
├── Migrations/
└── README.md
```

## Database
- **PostgreSQL 16** with **pgvector** extension
- Main tables:
  - pizzas (with embedding vectors)
  - orders
  - order_items
  - customers
  - chat_sessions
  - chat_messages

## Useful Commands
```bash
# Create project
dotnet new classlib -n KernelMind.Infrastructure

# Add reference
dotnet add reference ../KernelMind.Domain

# Add NuGet packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Pgvector.EntityFrameworkCore
dotnet add package Microsoft.Extensions.AI.Ollama
```
