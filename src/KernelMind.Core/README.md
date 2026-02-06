# KernelMind.Core

## 📋 Propósito
Camada de aplicação contendo a lógica de negócio, plugins do Semantic Kernel e orquestração.

## 📦 Responsabilidades
- **Plugins do Semantic Kernel:**
  - MenuPlugin (consulta cardápio)
  - OrderPlugin (gerenciamento de pedidos)
  - CalculationPlugin (cálculos de valores)
  - ContextPlugin (contexto da conversa)
- **Serviços de aplicação:**
  - ChatService (orquestração do chat)
  - OrderService (processamento de pedidos)
  - EmbeddingService (RAG - geração de embeddings)
- **DTOs e modelos de aplicação**

## 🔗 Referências
- KernelMind.Domain

## 📁 Estrutura Esperada
```
KernelMind.Core/
├── Plugins/
│   ├── MenuPlugin.cs
│   ├── OrderPlugin.cs
│   ├── CalculationPlugin.cs
│   └── ContextPlugin.cs
├── Services/
│   ├── ChatService.cs
│   ├── OrderService.cs
│   └── EmbeddingService.cs
├── DTOs/
│   ├── ChatRequest.cs
│   ├── ChatResponse.cs
│   └── OrderDto.cs
└── README.md
```

## 🚀 Comandos Úteis
```bash
# Criar projeto
dotnet new classlib -n KernelMind.Core

# Adicionar referência
dotnet add reference ../KernelMind.Domain

# Adicionar pacotes NuGet
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Plugins.Core
dotnet add package Microsoft.Extensions.AI.Abstractions
```
