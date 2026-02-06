# KernelMind.Api

## Purpose
ASP.NET 10 Web API project - Application entry point.
Contains controllers, middlewares, configuration, and dependency injection.

## Responsibilities
- REST Controllers for API endpoints
- Semantic Kernel configuration
- HTTP Streaming configuration (IAsyncEnumerable)
- Middlewares (logging, CORS, etc.)
- Swagger/OpenAPI documentation
- Health checks

## References
- KernelMind.Core
- KernelMind.Domain
- KernelMind.Infrastructure

## Expected Structure
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

## Useful Commands
```bash
# Create project
dotnet new webapi -n KernelMind.Api

# Add references
dotnet add reference ../KernelMind.Core
dotnet add reference ../KernelMind.Domain
dotnet add reference ../KernelMind.Infrastructure

# Add NuGet packages
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.Ollama
dotnet add package Swashbuckle.AspNetCore
```
