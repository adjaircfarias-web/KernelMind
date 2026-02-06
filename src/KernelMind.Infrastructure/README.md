# KernelMind.Infrastructure

## 📋 Propósito
Camada de infraestrutura contendo implementações concretas de acesso a dados, integrações externas e serviços de infraestrutura.

## 📦 Responsabilidades
- **Data Access:**
  - AppDbContext (Entity Framework Core)
  - Migrations
  - Configurações de entidades
- **Repositories:**
  - PizzaRepository
  - OrderRepository
  - ChatSessionRepository
- **Integrações Externas:**
  - OllamaClient (comunicação com LLM local)
  - VectorStore (pgvector para RAG)
- **Serviços de Infraestrutura:**
  - CacheService
  - LoggingService

## 🔗 Referências
- KernelMind.Domain

## 📁 Estrutura Esperada
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
│   ├── VectorStoreService.cs
│   └── CacheService.cs
├── Migrations/
└── README.md
```

## 🗄️ Banco de Dados
- **PostgreSQL 16** com extensão **pgvector**
- Tabelas principais:
  - pizzas (com vetores de embedding)
  - orders
  - order_items
  - customers
  - chat_sessions
  - chat_messages

## 🚀 Comandos Úteis
```bash
# Criar projeto
dotnet new classlib -n KernelMind.Infrastructure

# Adicionar referência
dotnet add reference ../KernelMind.Domain

# Adicionar pacotes NuGet
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Pgvector.EntityFrameworkCore
```
