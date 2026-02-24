# 📋 User Stories - KernelMind

**User Stories for System Implementation**

---

## 🎯 User Story Structure

Each US follows the format:
- **ID**: Unique identifier
- **Title**: Descriptive name
- **As**: User role
- **I want**: Desired functionality
- **So that**: Benefit/value
- **Acceptance Criteria**: Technical and functional requirements
- **Dependencies**: Other required US
- **Estimate**: Approximate time
- **Priority**: High/Medium/Low

---

## 📝 Code Standards

### Source Code Language
**All project source code must be written in English**, including:

- **Class names**: `Pizza`, `Order`, `ChatService` (not `Pedido`, `ServicoChat`)
- **Method names**: `GetMenu()`, `AddItem()`, `CalculateTotal()` (not `GetCardapio()`, `AdicionarItem()`)
- **Variable names**: `customerName`, `orderItems`, `totalPrice` (not `nomeCliente`, `itensPedido`)
- **Property names**: `Id`, `Name`, `Description` (not `Id`, `Nome`, `Descricao`)
- **File names**: `Pizza.cs`, `OrderController.cs` (not `Pizza.cs`, `PedidoController.cs`)
- **Table names**: `pizzas`, `orders`, `customers` (not `pizzas`, `pedidos`, `clientes`)
- **Column names**: `name`, `price`, `description` (not `nome`, `preco`, `descricao`)
- **Comments**: May be in Portuguese for team understanding
- **Documentation**: README, documentation comments (XML docs), may be in Portuguese

### Naming Examples

#### ❌ Incorrect (Portuguese)
```csharp
public class Pedido
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public decimal CalcularTotal() { }
}
```

#### ✅ Correct (English + `record`)
```csharp
public record Order
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public List<OrderItem> Items { get; init; } = new();
    
    public decimal CalculateTotal() => Items.Sum(i => i.Quantity * i.UnitPrice);
}

public record OrderItem
{
    public Guid PizzaId { get; init; }
    public string PizzaName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
```

### Exceptions
- **LLM prompts**: May be in Portuguese to improve response quality
- **User-facing error messages**: Should be in Portuguese (system language)
- **Logs**: Preferably in English, but may include Portuguese context

### Use of `record` vs `class`

**Prefer `record` over `class` where possible**, especially for:

✅ **Use `record`:**
- Simple domain entities (DTOs, Value Objects)
- API request/response models
- Immutable objects or those without complex behavior
- Types that need value-based equality

❌ **Use `class`:**
- Services (dependency injection)
- Plugins (need state or complex methods)
- Controllers
- DbContext
- Repositories
- Any class with significant behavior or mutable state

### Usage Examples

#### ✅ `record` for Entities/DTOs
```csharp
// Simple entities - immutable
public record Pizza(Guid Id, string Name, string Description, decimal Price, string Category);

public record OrderItem(Guid PizzaId, string PizzaName, int Quantity, decimal UnitPrice);

// API DTOs
public record MessageRequest(string Message, string? SessionId);

public record MessageResponse(string Content, DateTime Timestamp);

// Value Objects
public record Money(decimal Amount, string Currency);
```

#### ✅ `class` for Services and Plugins
```csharp
// Services need dependency injection
public class ChatService
{
    private readonly Kernel _kernel;
    
    public ChatService(Kernel kernel) { _kernel = kernel; }
    
    public async Task<string> ProcessMessageAsync(string message) { }
}

// Plugins need state or complex methods
public class MenuPlugin
{
    private readonly IPizzaRepository _repository;
    
    [KernelFunction("get_menu")]
    public async Task<string> GetMenuAsync() { }
}
```

#### ✅ `record` for Entities with Relationships
```csharp
// Entities that need collections
public record Order
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public List<OrderItem> Items { get; init; } = new();
    public decimal Total { get; init; }
    
    // Simple behavior can be a method on the record
    public decimal CalculateTotal() => Items.Sum(i => i.Quantity * i.UnitPrice);
}

public record OrderItem
{
    public Guid PizzaId { get; init; }
    public string PizzaName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string? Notes { get; init; }
}
```

---

## 📦 PHASE 0: Setup and Infrastructure

### US-001: Configure Project Folder Structure
**As** a developer  
**I want** an organized folder structure  
**So that** development and code maintenance are easier

**Acceptance Criteria:**
- [ ] Criar pasta `src/` com subpastas: Api, Core, Domain, Infrastructure, Web
- [ ] Criar pasta `docker/` com subpastas: postgres, ollama, nginx
- [ ] Criar pasta `scripts/`
- [ ] Criar pasta `docs/`
- [ ] Criar pasta `tests/`
- [ ] Criar arquivos raiz: README.md, .gitignore, .env.example

**Dependencies:** None
**Estimate:** 30 minutes
**Priority:** 🔴 High

---

### US-002: Configure Full Docker Compose
**As** a developer  
**I want** a working docker-compose.yml  
**So that** I can bring up the whole infrastructure with one command

**Acceptance Criteria:**
- [ ] Criar serviço `frontend` (Angular) na porta 4200
- [ ] Criar serviço `backend` (.NET) na porta 5076
- [ ] Criar serviço `postgres` (PostgreSQL + pgvector) na porta 5432
- [ ] Criar serviço `ollama` (LLM) na porta 11434
- [ ] Configurar rede compartilhada `kernelmind-network`
- [ ] Configurar volumes persistentes para postgres e ollama
- [ ] Adicionar healthchecks para postgres
- [ ] Criar docker-compose.override.yml para desenvolvimento

**Dependencies:** US-001
**Estimate:** 2 hours
**Priority:** 🔴 High

---

### US-003: Criar Dockerfiles para Todos os Serviços
**As** desenvolvedor  
**I want** ter Dockerfiles otimizados  
**So that** builds eficientes em produção e desenvolvimento

**Acceptance Criteria:**
- [ ] Criar `src/KernelMind.Web/Dockerfile` com multi-stage (build + nginx)
- [ ] Criar `src/KernelMind.Api/Dockerfile` com multi-stage (.NET)
- [ ] Criar `docker/postgres/Dockerfile` com pgvector instalado
- [ ] Criar `docker/ollama/Dockerfile` base
- [ ] Todos os Dockerfiles devem usar cache eficiente
- [ ] Stages de desenvolvimento e produção separados

**Dependencies:** US-001
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-004: Configurar Variáveis de Ambiente
**As** desenvolvedor  
**I want** ter um arquivo .env configurável  
**So that** gerenciar configurações sensíveis e de ambiente

**Acceptance Criteria:**
- [ ] Criar `.env.example` com todas as variáveis documentadas
- [ ] Configurar `POSTGRES_PASSWORD`, `POSTGRES_DB`, `POSTGRES_USER`
- [ ] Configurar OLLAMA_URL, OLLAMA_MODEL
- [ ] Configurar ASPNETCORE_ENVIRONMENT
- [ ] Configurar API_URL para frontend
- [ ] Adicionar `.env` ao .gitignore
- [ ] Documentar todas as variáveis no README

**Dependencies:** US-002
**Estimate:** 1 hora
**Priority:** 🟡 Medium

---

### US-005: Criar Scripts de Setup e Utilitários
**As** desenvolvedor  
**I want** ter scripts para automatizar tarefas comuns  
**So that** facilitar o setup e operação do projeto

**Acceptance Criteria:**
- [ ] Criar `scripts/setup.ps1` (setup inicial Windows)
- [ ] Criar `scripts/setup.sh` (setup inicial Linux/Mac)
- [ ] Criar `Makefile` com comandos: up, down, build, logs, seed, clean
- [ ] Scripts devem verificar pré-requisitos (Docker, Ollama)
- [ ] Scripts devem criar .env automaticamente se não existir
- [ ] Adicionar mensagens coloridas e informativas

**Dependencies:** US-002, US-004
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

## 📦 PHASE 1: Backend Core (Domain and Data)

### US-006: Criar Projetos .NET 10
**As** desenvolvedor  
**I want** ter a solution e projetos configurados  
**So that** começar o desenvolvimento do backend

**Acceptance Criteria:**
- [ ] Criar `KernelMind.sln` na raiz
- [ ] Criar projeto `KernelMind.Domain` (Class Library)
- [ ] Criar projeto `KernelMind.Infrastructure` (Class Library)
- [ ] Criar projeto `KernelMind.Core` (Class Library)
- [ ] Criar projeto `KernelMind.Api` (Web API)
- [ ] Configurar referências entre projetos
- [ ] Adicionar pacotes NuGet iniciais

**Dependencies:** US-001
**Estimate:** 1 hora
**Priority:** 🔴 High

---

### US-007: Implementar Entidades do Domínio
**As** desenvolvedor  
**I want** ter as entidades principais definidas  
**So that** representar os dados do sistema

**Acceptance Criteria:**
- [ ] Criar `Pizza.cs` como `record` com: Id, Name, Description, Ingredients, Price, Category, Embedding
- [ ] Criar `Order.cs` como `record` com: Id, CustomerName, Phone, Address, Items, Total, Status
- [ ] Criar `OrderItem.cs` como `record` com: PizzaId, PizzaName, Quantity, UnitPrice, Notes
- [ ] Criar `Conversation.cs` como `record` com: Id, SessionId, Role, Content, Timestamp
- [ ] Todas as entidades devem ter Guid como chave primária
- [ ] Configurar data annotations ou fluent validation

**Dependencies:** US-006
**Estimate:** 2 horas
**Priority:** 🔴 High

---

### US-008: Configurar Entity Framework Core
**As** desenvolvedor  
**I want** ter o DbContext configurado  
**So that** acessar o banco de dados PostgreSQL

**Acceptance Criteria:**
- [ ] Criar `AppDbContext.cs` em Infrastructure
- [ ] Configurar DbSets para todas as entidades
- [ ] Configurar string de conexão via appsettings.json
- [ ] Adicionar pacotes: Npgsql.EntityFrameworkCore.PostgreSQL, Pgvector.EntityFrameworkCore
- [ ] Configurar suporte a vetores (pgvector) no DbContext
- [ ] Configurar logging do EF Core

**Dependencies:** US-007
**Estimate:** 2 horas
**Priority:** 🔴 High

---

### US-009: Criar Primeiras Migrations
**As** desenvolvedor  
**I want** ter o schema do banco versionado  
**So that** criar as tabelas inicialmente

**Acceptance Criteria:**
- [ ] Criar migration `InitialCreate`
- [ ] Migration deve criar tabelas: pizzas, orders, order_items, conversations
- [ ] Configurar índice vetorial para tabela Pizzas
- [ ] Aplicar migration com `dotnet ef database update`
- [ ] Verificar se tabelas foram criadas corretamente

**Dependencies:** US-008
**Estimate:** 1 hora
**Priority:** 🔴 High

---

### US-010: Implementar Repositórios
**As** desenvolvedor  
**I want** ter a camada de acesso a dados  
**So that** abstrair as operações CRUD

**Acceptance Criteria:**
- [ ] Criar interface `IPizzaRepository` e implementação
- [ ] Criar interface `IOrderRepository` e implementação
- [ ] Criar interface `IConversationRepository` e implementação
- [ ] Implementar métodos básicos: GetAll, GetById, Add, Update, Delete
- [ ] Implementar métodos específicos: GetByName (Pizza), GetBySessao (Conversa)
- [ ] Injetar DbContext nos repositórios
- [ ] Configurar injeção de dependência no Program.cs

**Dependencies:** US-009
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-011: Criar Seed Data do Cardápio
**As** desenvolvedor  
**I want** ter dados iniciais de pizzas  
**So that** popular o banco para testes

**Acceptance Criteria:**
- [ ] Criar classe `SeedData.cs` com 15+ pizzas
- [ ] Incluir pizzas de diferentes categorias: Tradicional, Especial, Doce
- [ ] Criar comando CLI: `dotnet run --seed`
- [ ] Seed deve verificar duplicatas antes de inserir
- [ ] Documentar pizzas no README
- [ ] Testar seed e verificar se pizzas foram inseridas

**Dependencies:** US-010
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

## 📦 PHASE 2: Semantic Kernel and Plugins

### US-012: Configurar Semantic Kernel
**As** desenvolvedor  
**I want** ter o Semantic Kernel configurado  
**So that** integrar com o Ollama local

**Acceptance Criteria:**
- [ ] Instalar pacotes: Microsoft.SemanticKernel, Microsoft.SemanticKernel.Connectors.Ollama
- [ ] Criar `KernelConfig.cs` com configuração do Kernel
- [ ] Configurar ChatCompletion com modelo llama3.1:70b
- [ ] Configurar TextEmbeddingGeneration com nomic-embed-text
- [ ] Configurar Ollama URL via appsettings
- [ ] Testar conexão com Ollama (health check)
- [ ] Criar serviço injetável: `IKernelService`

**Dependencies:** US-006
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-013: Implementar MenuPlugin
**As** usuário  
**I want** consultar o cardápio via chat  
**So that** ver as pizzas disponíveis

**Acceptance Criteria:**
- [ ] Criar `MenuPlugin.cs` com atributo `[KernelFunction]`
- [ ] Implementar função `list_menu`: lista todas as pizzas
- [ ] Implementar função `search_pizza`: busca por nome ou ingrediente
- [ ] Implementar função `get_pizza_details`: mostra detalhes de uma pizza
- [ ] Funções devem usar IPizzaRepository
- [ ] Formatar resposta de forma amigável (com emojis)
- [ ] Testar plugin isoladamente

**Dependencies:** US-010, US-012
**Estimate:** 4 horas
**Priority:** 🔴 High

---

### US-014: Implementar PedidoPlugin
**As** usuário  
**I want** adicionar pizzas ao pedido  
**So that** montar meu pedido via chat

**Acceptance Criteria:**
- [ ] Criar `OrderPlugin.cs`
- [ ] Implementar função `add_item`: adiciona pizza ao pedido atual
- [ ] Implementar função `remove_item`: remove pizza do pedido
- [ ] Implementar função `view_order`: mostra pedido atual
- [ ] Implementar função `confirm_order`: salva pedido no banco
- [ ] Implementar função `cancel_order`: limpa pedido atual
- [ ] Manter estado do pedido em memória (sessão)
- [ ] Validar se pizza existe antes de adicionar
- [ ] Testar ciclo completo: adicionar → ver → confirmar

**Dependencies:** US-013
**Estimate:** 5 horas
**Priority:** 🔴 High

---

### US-015: Implementar CalculoPlugin
**As** usuário  
**I want** calcular valores e ver promoções  
**So that** saber o preço do pedido

**Acceptance Criteria:**
- [ ] Criar `CalculationPlugin.cs`
- [ ] Implementar função `calculate_total`: soma valores dos itens
- [ ] Implementar função `apply_discount`: aplica % de desconto
- [ ] Implementar função `calculate_delivery_fee`: retorna taxa por bairro
- [ ] Implementar função `check_promotion`: mostra promoção do dia
- [ ] Criar dicionário de taxas por bairro
- [ ] Criar lógica de promoções por dia da semana
- [ ] Testar cálculos com diferentes cenários

**Dependencies:** US-014
**Estimate:** 3 horas
**Priority:** 🟡 Medium

---

### US-016: Implementar ContextoPlugin
**As** usuário  
**I want** que o bot lembre da conversa  
**So that** ter contexto nas respostas

**Acceptance Criteria:**
- [ ] Criar `ContextPlugin.cs`
- [ ] Implementar função `save_message`: salva no banco
- [ ] Implementar função `get_history`: busca últimas N mensagens
- [ ] Implementar função `clear_context`: remove histórico
- [ ] Usar IConversaRepository
- [ ] Limitar histórico às últimas 10 mensagens
- [ ] Formatar histórico para prompt do LLM

**Dependencies:** US-010
**Estimate:** 3 horas
**Priority:** 🟡 Medium

---

### US-017: Criar ChatService
**As** desenvolvedor  
**I want** ter um serviço de chat unificado  
**So that** orquestrar plugins e LLM

**Acceptance Criteria:**
- [ ] Criar `ChatService.cs`
- [ ] Injetar Kernel e todos os plugins
- [ ] Criar método `ProcessMessageAsync`: processa uma mensagem
- [ ] Criar método `StreamChatAsync`: processa com streaming (IAsyncEnumerable)
- [ ] Implementar pipeline: histórico → LLM → resposta
- [ ] Configurar prompt system para atendente de pizzaria
- [ ] Testar integração com todos os plugins

**Dependencies:** US-013, US-014, US-015, US-016
**Estimate:** 4 horas
**Priority:** 🔴 High

---

## 📦 PHASE 3: RAG and Embeddings

### US-018: Configurar pgvector no Banco
**As** desenvolvedor  
**I want** ter suporte a vetores no PostgreSQL  
**So that** armazenar embeddings das pizzas

**Acceptance Criteria:**
- [ ] Verificar se extensão pgvector está instalada
- [ ] Criar migration para adicionar coluna `embedding` (tipo vector)
- [ ] Criar índice ivfflat para busca vetorial
- [ ] Testar inserção de vetores manualmente
- [ ] Documentar dimensões do vetor (1536 para nomic-embed-text)

**Dependencies:** US-009
**Estimate:** 1 hora
**Priority:** 🔴 High

---

### US-019: Implementar EmbeddingService
**As** desenvolvedor  
**I want** gerar embeddings de textos  
**So that** vetorizar pizzas e consultas

**Acceptance Criteria:**
- [ ] Criar `EmbeddingService.cs`
- [ ] Injetar ITextEmbeddingGenerationService
- [ ] Implementar `GenerateEmbeddingAsync(Pizza)`: gera vetor da pizza
- [ ] Implementar `GenerateQueryEmbeddingAsync(string)`: gera vetor da consulta
- [ ] Formatar texto da pizza: nome + descrição + ingredientes
- [ ] Retornar tipo Vector do pgvector
- [ ] Testar geração de embeddings

**Dependencies:** US-012, US-018
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-020: Implementar VectorSearchService
**As** usuário  
**I want** buscar pizzas por similaridade  
**So that** encontrar pizzas por descrição

**Acceptance Criteria:**
- [ ] Criar `VectorSearchService.cs`
- [ ] Injetar AppDbContext e EmbeddingService
- [ ] Implementar `SearchAsync(string query, int topK)`: busca semântica
- [ ] Usar cosine similarity do pgvector
- [ ] Ordenar por distância vetorial (mais similar primeiro)
- [ ] Retornar top-K resultados (padrão: 3)
- [ ] Testar busca: "pizza com bacon" deve retornar pizzas com bacon
- [ ] Testar busca: "doce" deve retornar pizzas doces

**Dependencies:** US-019
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-021: Criar Pipeline de Vetorização do Cardápio
**As** desenvolvedor  
**I want** vetorizar todas as pizzas  
**So that** habilitar busca semântica

**Acceptance Criteria:**
- [ ] Criar comando: `dotnet run --vectorize`
- [ ] Implementar `IndexAllPizzasAsync` no EmbeddingService
- [ ] Buscar todas as pizzas sem embedding
- [ ] Gerar embedding para cada uma
- [ ] Salvar embedding no banco
- [ ] Mostrar progresso no console
- [ ] Verificar se todas as pizzas foram vetorizadas

**Dependencies:** US-020
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

### US-022: Integrar RAG no MenuPlugin
**As** usuário  
**I want** buscar pizzas semanticamente  
**So that** encontrar opções por descrição

**Acceptance Criteria:**
- [ ] Atualizar `MenuPlugin.buscar_pizza` para usar VectorSearchService
- [ ] Quando termo não encontra match exato, usar busca vetorial
- [ ] Retornar top 3 resultados mais relevantes
- [ ] Formatar resposta com score de similaridade (opcional)
- [ ] Testar: "algo picante" → deve retornar pepperoni, 4 queijos
- [ ] Testar: "leve" → deve retornar margherita

**Dependencies:** US-021
**Estimate:** 2 horas
**Priority:** 🔴 High

---

## 📦 PHASE 4: REST API with Streaming

### US-023: Criar Controllers da API
**As** desenvolvedor  
**I want** expor endpoints REST  
**So that** comunicação com frontend

**Acceptance Criteria:**
- [ ] Criar `ChatController` com endpoint POST /api/chat
- [ ] Criar `MenuController` com endpoints: GET /api/menu, GET /api/menu/buscar
- [ ] Criar `PedidoController` com endpoints: POST /api/pedidos, GET /api/pedidos/{id}
- [ ] Configurar routing e atributos
- [ ] Configurar Swagger/OpenAPI
- [ ] Adicionar tratamento de erros global
- [ ] Configurar CORS para frontend Angular

**Dependencies:** US-017, US-022
**Estimate:** 4 horas
**Priority:** 🔴 High

---

### US-024: Implementar Endpoint de Chat com Streaming
**As** usuário  
**I want** ver a resposta sendo digitada  
**So that** ter experiência de chat em tempo real

**Acceptance Criteria:**
- [ ] Criar endpoint POST /api/chat/stream
- [ ] Retornar `IAsyncEnumerable<string>`
- [ ] Usar `StreamChatAsync` do ChatService
- [ ] Configurar Content-Type: text/event-stream ou application/json+stream
- [ ] Usar `yield return` para cada token
- [ ] Forçar flush com `await Task.Yield()`
- [ ] Configurar CancellationToken para cancelar stream
- [ ] Testar com curl: `curl -N -X POST http://localhost:5076/api/chat/stream`

**Dependencies:** US-023
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-025: Criar DTOs e Validações
**As** desenvolvedor  
**I want** ter contratos de API bem definidos  
**So that** tipagem no frontend

**Acceptance Criteria:**
- [ ] Criar `MessageRequest` como `record` (message, sessionId)
- [ ] Criar `MessageResponse` como `record` (content, timestamp)
- [ ] Criar `PizzaDto` como `record` (id, name, description, price, ingredients)
- [ ] Criar `OrderRequest` como `record` (customerName, phone, address, items)
- [ ] Criar `OrderResponse` como `record` (id, number, total, status)
- [ ] Adicionar validações com FluentValidation ou DataAnnotations
- [ ] Retornar erros 400 com detalhes de validação

**Dependencies:** US-023
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

### US-026: Documentar API com Swagger
**As** desenvolvedor  
**I want** ter documentação interativa da API  
**So that** facilitar testes e integração

**Acceptance Criteria:**
- [ ] Configurar Swagger UI em /swagger
- [ ] Documentar todos os endpoints
- [ ] Adicionar exemplos de request/response
- [ ] Documentar códigos de erro possíveis
- [ ] Adicionar descrições em português
- [ ] Configurar Swagger para ambiente de desenvolvimento
- [ ] Testar todos os endpoints via Swagger UI

**Dependencies:** US-025
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

## 📦 PHASE 5: Angular Frontend

### US-027: Criar Projeto Angular 19
**As** desenvolvedor  
**I want** ter o projeto frontend configurado  
**So that** desenvolver a interface

**Acceptance Criteria:**
- [ ] Criar projeto com `ng new KernelMind.Web --routing --style=scss`
- [ ] Configurar strict mode
- [ ] Instalar Angular Material: `ng add @angular/material`
- [ ] Configurar tema escuro/claro
- [ ] Estruturar pastas: components, services, models
- [ ] Configurar environments (dev/prod)
- [ ] Criar proxy.conf.json para desenvolvimento

**Dependencies:** None (pode ser feito em paralelo)
**Estimate:** 2 horas
**Priority:** 🔴 High

---

### US-028: Criar Models TypeScript
**As** desenvolvedor  
**I want** ter as interfaces de dados  
**So that** tipagem forte no frontend

**Acceptance Criteria:**
- [ ] Criar `pizza.model.ts`: interface Pizza (TypeScript não tem record, usar readonly quando possível)
- [ ] Criar `order.model.ts`: interfaces Order, OrderItem
- [ ] Criar `message.model.ts`: interface ChatMessage
- [ ] Todos os campos devem ser tipados
- [ ] Adicionar enums para StatusPedido, CategoriaPizza

**Dependencies:** US-027
**Estimate:** 1 hora
**Priority:** 🟡 Medium

---

### US-029: Implementar ChatService
**As** desenvolvedor  
**I want** comunicar com backend via HTTP  
**So that** enviar mensagens e receber respostas

**Acceptance Criteria:**
- [ ] Criar `ChatService` injetável
- [ ] Implementar `enviarMensagem(mensagem)`: POST simples
- [ ] Implementar `obterCardapio()`: GET /api/menu
- [ ] Implementar `buscarPizzas(termo)`: GET /api/menu/buscar
- [ ] Configurar base URL via environment
- [ ] Tratar erros HTTP com mensagens amigáveis

**Dependencies:** US-028
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-030: Implementar StreamingService
**As** usuário  
**I want** ver a resposta sendo construída  
**So that** ter experiência fluida de chat

**Acceptance Criteria:**
- [ ] Criar `StreamingService`
- [ ] Implementar `enviarMensagemStream(mensagem, onChunk)`
- [ ] Usar Fetch API nativa (não Angular HttpClient)
- [ ] Ler stream com `response.body.getReader()`
- [ ] Decodificar chunks com `TextDecoder`
- [ ] Chamar callback `onChunk` para cada pedaço recebido
- [ ] Tratamento de erros de conexão
- [ ] Suporte a cancelamento (AbortController)

**Dependencies:** US-029
**Estimate:** 4 horas
**Priority:** 🔴 High

---

### US-031: Implementar ChatComponent
**As** usuário  
**I want** uma interface de chat intuitiva  
**So that** conversar com o bot

**Acceptance Criteria:**
- [ ] Criar `ChatComponent` com selector `app-chat`
- [ ] Layout: header, message area, input
- [ ] Mensagens com diferenciação visual (user vs bot)
- [ ] Suporte a markdown/html nas mensagens do bot
- [ ] Scroll automático para última mensagem
- [ ] Indicador de "typing..."
- [ ] Input com Enter para enviar
- [ ] Botão enviar desabilitado quando vazio
- [ ] Usar Angular Material (MatInput, MatButton, MatCard)

**Dependencies:** US-030
**Estimate:** 5 horas
**Priority:** 🔴 High

---

### US-032: Criar Componente de Pedido
**As** usuário  
**I want** ver meu pedido atual  
**So that** acompanhar o que estou comprando

**Acceptance Criteria:**
- [ ] Criar `OrderComponent`
- [ ] Mostrar lista de itens com quantidade e preço
- [ ] Mostrar total do pedido
- [ ] Botão para remover item
- [ ] Botão para confirmar pedido (abre diálogo)
- [ ] Botão para cancelar pedido
- [ ] Atualizar em tempo real quando itens são adicionados

**Dependencies:** US-031
**Estimate:** 4 horas
**Priority:** 🟡 Medium

---

### US-033: Implementar Tema Visual
**As** usuário  
**I want** uma interface bonita  
**So that** melhor experiência

**Acceptance Criteria:**
- [ ] Configurar tema com cores da pizzaria (vermelho, amarelo)
- [ ] Criar variáveis SCSS para cores
- [ ] Estilizar ChatComponent (bubbles, avatares)
- [ ] Adicionar animações (fade in das mensagens)
- [ ] Tornar responsivo (mobile-friendly)
- [ ] Adicionar favicon e título da página
- [ ] Adicionar emojis relacionados a pizza 🍕

**Dependencies:** US-031
**Estimate:** 4 horas
**Priority:** 🟡 Medium

---

### US-034: Configurar Proxy e Ambientes
**As** desenvolvedor  
**I want** comunicar frontend com backend  
**So that** desenvolvimento sem CORS

**Acceptance Criteria:**
- [ ] Criar `proxy.conf.json` para redirecionar /api para localhost:5076
- [ ] Configurar angular.json para usar proxy
- [ ] Criar environments com URLs da API
- [ ] Configurar produção para usar nginx proxy
- [ ] Testar comunicação end-to-end
- [ ] Verificar se streaming funciona via proxy

**Dependencies:** US-030
**Estimate:** 2 horas
**Priority:** 🟡 Medium

---

## 📦 PHASE 6: Integration and Deploy

### US-035: Configurar Docker Compose Completo
**As** desenvolvedor  
**I want** subir toda a aplicação  
**So that** testar integração

**Acceptance Criteria:**
- [ ] Atualizar docker-compose.yml com builds dos projetos
- [ ] Configurar depends_on entre serviços
- [ ] Testar `docker-compose up --build`
- [ ] Verificar se todos os containers iniciam
- [ ] Testar comunicação entre containers
- [ ] Verificar se frontend acessa backend via nginx

**Dependencies:** Todas as anteriores
**Estimate:** 3 horas
**Priority:** 🔴 High

---

### US-036: Testar Fluxo End-to-End
**As** QA/Tester  
**I want** validar o sistema completo  
**So that** garantir que tudo funciona

**Acceptance Criteria:**
- [ ] Cenário 1: Usuário abre chat, vê saudação
- [ ] Cenário 2: Usuário pede cardápio, vê lista de pizzas
- [ ] Cenário 3: Usuário busca "bacon", vê resultados relevantes
- [ ] Cenário 4: Usuário adiciona pizzas ao pedido
- [ ] Cenário 5: Usuário confirma pedido, recebe número
- [ ] Cenário 6: Streaming funciona (resposta aparece gradativamente)
- [ ] Testar em diferentes navegadores (Chrome, Firefox)
- [ ] Testar em mobile (viewport reduzido)

**Dependencies:** US-035
**Estimate:** 4 horas
**Priority:** 🔴 High

---

### US-037: Otimizar Performance
**As** desenvolvedor  
**I want** melhorar velocidade  
**So that** melhor UX

**Acceptance Criteria:**
- [ ] Verificar bundle size do Angular (lazy loading)
- [ ] Configurar caching do nginx
- [ ] Otimizar imagens (se houver)
- [ ] Verificar tempo de resposta do backend (< 2s para queries)
- [ ] Adicionar gzip compression
- [ ] Testar com Lighthouse (score > 80)

**Dependencies:** US-036
**Estimate:** 3 horas
**Priority:** 🟢 Low

---

### US-038: Criar Documentação Final
**As** desenvolvedor  
**I want** documentar o projeto  
**So that** outros desenvolvedores

**Acceptance Criteria:**
- [ ] Atualizar README.md com instruções completas
- [ ] Criar diagrama de arquitetura
- [ ] Documentar como adicionar novas pizzas
- [ ] Documentar como treinar modelo RAG
- [ ] Criar vídeo demo (opcional)
- [ ] Documentar troubleshooting comum

**Dependencies:** Todas
**Estimate:** 4 horas
**Priority:** 🟢 Low

---

### US-039: Criar Testes Automatizados
**As** desenvolvedor  
**I want** ter testes automatizados  
**So that** garantir qualidade

**Acceptance Criteria:**
- [ ] Testes unitários para Plugins (xUnit)
- [ ] Testes de integração para API
- [ ] Testes E2E para fluxo principal (Cypress ou Playwright)
- [ ] Cobertura mínima: 70%
- [ ] Pipeline de CI/CD (GitHub Actions)
- [ ] Rodar testes no `docker-compose` de testes

**Dependencies:** Todas
**Estimate:** 8 horas
**Priority:** 🟢 Low

---

## 📊 Resumo e Métricas

### Total de User Stories: 39

| Fase | Quantidade | Estimativa Total |
|------|-----------|------------------|
| Fase 0 - Setup | 5 | 8 horas |
| Fase 1 - Domínio | 6 | 11 horas |
| Fase 2 - Semantic Kernel | 6 | 22 horas |
| Fase 3 - RAG | 5 | 11 horas |
| Fase 4 - API | 4 | 11 horas |
| Fase 5 - Frontend | 8 | 25 horas |
| Fase 6 - Integração | 5 | 22 horas |
| **TOTAL** | **39** | **~110 horas** |

### Prioridades
- 🔴 **High**: 21 US (54%)
- 🟡 **Medium**: 13 US (33%)
- 🟢 **Low**: 5 US (13%)

### Timeline Sugerida (20 dias)
- Semana 1: Fases 0-1 (Setup + Domínio)
- Semana 2: Fases 2-3 (Semantic Kernel + RAG)
- Semana 3: Fases 4-5 (API + Frontend)
- Semana 4: Fase 6 (Integração + Testes)

---

## 🎯 Critérios de Aceitação Gerais do Projeto

O projeto estará completo quando:

✅ **Funcionalidades Core:**
- [ ] Chatbot responde em linguagem natural
- [ ] Busca semântica (RAG) funciona
- [ ] Plugins respondem corretamente
- [ ] Pedidos são salvos no banco

✅ **Integração:**
- [ ] Frontend comunica com backend
- [ ] Streaming de respostas funciona
- [ ] Docker Compose sobe tudo com um comando

✅ **Qualidade:**
- [ ] Código bem estruturado e documentado
- [ ] Testes automatizados passando
- [ ] README completo

✅ **Demo:**
- [ ] Demonstração funcional do fluxo completo
- [ ] Vídeo ou screenshots documentando

---

**Documento de User Stories - KernelMind**

*Versão: 1.2*  
*Data: 06/02/2026*  
*Total de US: 39*  
*Estimativa Total: ~110 horas (~14 dias de trabalho)*

---

### 📝 Notas sobre Atualizações

**Versão 1.1** - Atualizado em 06/02/2026:
- ✅ Adicionada seção "Padrões de Código" exigindo código fonte em inglês
- ✅ Atualizados todos os exemplos de nomenclatura para inglês
- ✅ Classes: `Order` (não `Pedido`), `Pizza`, `Customer`
- ✅ Métodos: `AddItem()` (não `AdicionarItem()`), `CalculateTotal()`
- ✅ Propriedades: `Name`, `Price`, `Description` (não `Nome`, `Preco`, `Descricao`)
- ✅ Tabelas: `orders`, `order_items` (não `pedidos`, `itens_pedido`)

**Versão 1.2** - Atualizado em 06/02/2026:
- ✅ Adicionada seção "Uso de `record` vs `class`"
- ✅ **Preferir `record` ao invés de `class`** para entidades, DTOs e Value Objects
- ✅ Entidades devem usar `init` setters ao invés de `set`
- ✅ Atualizadas todas as US para indicar uso de `record` onde apropriado
- ✅ Serviços, Plugins, Controllers e Repositories continuam como `class`

---

**Próximo Passo:** Iniciar implementação das US na ordem definida! 🚀
