# KernelMind.Api

## 📋 Propósito
Projeto ASP.NET 10 Web API - Ponto de entrada da aplicação.
Contém os controllers, middlewares, configurações e injeção de dependências.

## 📦 Responsabilidades
- Controllers REST para endpoints da API
- Configuração do Semantic Kernel
- Configuração do HTTP Streaming (IAsyncEnumerable)
- Middlewares (logging, CORS, etc.)
- Swagger/OpenAPI documentation
- Health checks

## 🔗 Referências
- KernelMind.Core
- KernelMind.Domain
- KernelMind.Infrastructure

## 📁 Estrutura Esperada
```
KernelMind.Api/
├── Controllers/
│   ├── ChatController.cs
│   ├── OrderController.cs
│   └── HealthController.cs
├── Middlewares/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## 🚀 Comandos Úteis
```bash
# Criar projeto
dotnet new webapi -n KernelMind.Api

# Adicionar referências
dotnet add reference ../KernelMind.Core
dotnet add reference ../KernelMind.Domain
dotnet add reference ../KernelMind.Infrastructure

# Adicionar pacotes NuGet
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Plugins.Core
dotnet add package Swashbuckle.AspNetCore
```
