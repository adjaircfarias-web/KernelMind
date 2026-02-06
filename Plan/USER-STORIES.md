# 📋 User Stories - KernelMind

**Histórias de Usuário para Implementação do Sistema**

---

## 🎯 Estrutura das User Stories

Cada US segue o formato:
- **ID**: Identificador único
- **Título**: Nome descritivo
- **Como**: Papel do usuário
- **Quero**: Funcionalidade desejada
- **Para**: Benefício/valor
- **Critérios de Aceitação**: Requisitos técnicos e funcionais
- **Dependências**: Outras US necessárias
- **Estimativa**: Tempo aproximado
- **Prioridade**: Alta/Média/Baixa

---

## 📝 Padrões de Código

### Idioma do Código Fonte
**Todo o código fonte do projeto deve ser escrito em inglês**, incluindo:

- **Nomes de classes**: `Pizza`, `Order`, `ChatService` (não `Pedido`, `ServicoChat`)
- **Nomes de métodos**: `GetMenu()`, `AddItem()`, `CalculateTotal()` (não `GetCardapio()`, `AdicionarItem()`)
- **Nomes de variáveis**: `customerName`, `orderItems`, `totalPrice` (não `nomeCliente`, `itensPedido`)
- **Nomes de propriedades**: `Id`, `Name`, `Description` (não `Id`, `Nome`, `Descricao`)
- **Nomes de arquivos**: `Pizza.cs`, `OrderController.cs` (não `Pizza.cs`, `PedidoController.cs`)
- **Nomes de tabelas**: `pizzas`, `orders`, `customers` (não `pizzas`, `pedidos`, `clientes`)
- **Nomes de colunas**: `name`, `price`, `description` (não `nome`, `preco`, `descricao`)
- **Comentários**: Podem ser em português para facilitar o entendimento da equipe
- **Documentação**: README, comentários de documentação (XML docs), podem ser em português

### Exemplos de Nomenclatura

#### ❌ Incorreto (Português)
```csharp
public class Pedido
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public decimal CalcularTotal() { }
}
```

#### ✅ Correto (Inglês + `record`)
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

### Exceções
- **Prompts do LLM**: Podem ser em português para melhorar a qualidade das respostas
- **Mensagens de erro para usuário**: Devem ser em português (idioma do sistema)
- **Logs**: Preferencialmente em inglês, mas podem ter contexto em português

### Uso de `record` vs `class`

**Preferir `record` ao invés de `class` onde for possível**, especialmente para:

✅ **Usar `record`:**
- Entidades de domínio simples (DTOs, Value Objects)
- Modelos de request/response da API
- Objetos imutáveis ou sem comportamento complexo
- Tipos que precisam de equality baseada em valor

❌ **Usar `class`:**
- Serviços (injeção de dependência)
- Plugins (precisam de estado ou métodos complexos)
- Controllers
- DbContext
- Repositories
- Qualquer classe com comportamento significativo ou estado mutável

### Exemplos de Uso

#### ✅ `record` para Entidades/DTOs
```csharp
// Entidades simples - imutáveis
public record Pizza(Guid Id, string Name, string Description, decimal Price, string Category);

public record OrderItem(Guid PizzaId, string PizzaName, int Quantity, decimal UnitPrice);

// DTOs para API
public record MessageRequest(string Message, string? SessionId);

public record MessageResponse(string Content, DateTime Timestamp);

// Value Objects
public record Money(decimal Amount, string Currency);
```

#### ✅ `class` para Serviços e Plugins
```csharp
// Serviços precisam de injeção de dependência
public class ChatService
{
    private readonly Kernel _kernel;
    
    public ChatService(Kernel kernel) { _kernel = kernel; }
    
    public async Task<string> ProcessMessageAsync(string message) { }
}

// Plugins precisam de estado ou métodos complexos
public class MenuPlugin
{
    private readonly IPizzaRepository _repository;
    
    [KernelFunction("get_menu")]
    public async Task<string> GetMenuAsync() { }
}
```

#### ✅ `record` para Entidades com Relacionamentos
```csharp
// Entidades que precisam de coleções
public record Order
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public List<OrderItem> Items { get; init; } = new();
    public decimal Total { get; init; }
    
    // Comportamento simples pode ser método no record
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

## 📦 FASE 0: Setup e Infraestrutura

### US-001: Configurar Estrutura de Pastas do Projeto
**Como** desenvolvedor  
**Quero** ter uma estrutura de pastas organizada  
**Para** facilitar o desenvolvimento e manutenção do código

**Critérios de Aceitação:**
- [ ] Criar pasta `src/` com subpastas: Api, Core, Domain, Infrastructure, Web
- [ ] Criar pasta `docker/` com subpastas: postgres, ollama, nginx
- [ ] Criar pasta `scripts/`
- [ ] Criar pasta `docs/`
- [ ] Criar pasta `tests/`
- [ ] Criar arquivos raiz: README.md, .gitignore, .env.example

**Dependências:** Nenhuma
**Estimativa:** 30 minutos
**Prioridade:** 🔴 Alta

---

### US-002: Configurar Docker Compose Completo
**Como** desenvolvedor  
**Quero** ter um docker-compose.yml funcional  
**Para** subir toda a infraestrutura com um comando

**Critérios de Aceitação:**
- [ ] Criar serviço `frontend` (Angular) na porta 4200
- [ ] Criar serviço `backend` (.NET) na porta 5076
- [ ] Criar serviço `postgres` (PostgreSQL + pgvector) na porta 5432
- [ ] Criar serviço `ollama` (LLM) na porta 11434
- [ ] Configurar rede compartilhada `kernelmind-network`
- [ ] Configurar volumes persistentes para postgres e ollama
- [ ] Adicionar healthchecks para postgres
- [ ] Criar docker-compose.override.yml para desenvolvimento

**Dependências:** US-001
**Estimativa:** 2 horas
**Prioridade:** 🔴 Alta

---

### US-003: Criar Dockerfiles para Todos os Serviços
**Como** desenvolvedor  
**Quero** ter Dockerfiles otimizados  
**Para** builds eficientes em produção e desenvolvimento

**Critérios de Aceitação:**
- [ ] Criar `src/KernelMind.Web/Dockerfile` com multi-stage (build + nginx)
- [ ] Criar `src/KernelMind.Api/Dockerfile` com multi-stage (.NET)
- [ ] Criar `docker/postgres/Dockerfile` com pgvector instalado
- [ ] Criar `docker/ollama/Dockerfile` base
- [ ] Todos os Dockerfiles devem usar cache eficiente
- [ ] Stages de desenvolvimento e produção separados

**Dependências:** US-001
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-004: Configurar Variáveis de Ambiente
**Como** desenvolvedor  
**Quero** ter um arquivo .env configurável  
**Para** gerenciar configurações sensíveis e de ambiente

**Critérios de Aceitação:**
- [ ] Criar `.env.example` com todas as variáveis documentadas
- [ ] Configurar `POSTGRES_PASSWORD`, `POSTGRES_DB`, `POSTGRES_USER`
- [ ] Configurar OLLAMA_URL, OLLAMA_MODEL
- [ ] Configurar ASPNETCORE_ENVIRONMENT
- [ ] Configurar API_URL para frontend
- [ ] Adicionar `.env` ao .gitignore
- [ ] Documentar todas as variáveis no README

**Dependências:** US-002
**Estimativa:** 1 hora
**Prioridade:** 🟡 Média

---

### US-005: Criar Scripts de Setup e Utilitários
**Como** desenvolvedor  
**Quero** ter scripts para automatizar tarefas comuns  
**Para** facilitar o setup e operação do projeto

**Critérios de Aceitação:**
- [ ] Criar `scripts/setup.ps1` (setup inicial Windows)
- [ ] Criar `scripts/setup.sh` (setup inicial Linux/Mac)
- [ ] Criar `Makefile` com comandos: up, down, build, logs, seed, clean
- [ ] Scripts devem verificar pré-requisitos (Docker, Ollama)
- [ ] Scripts devem criar .env automaticamente se não existir
- [ ] Adicionar mensagens coloridas e informativas

**Dependências:** US-002, US-004
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

## 📦 FASE 1: Backend Core (Domínio e Dados)

### US-006: Criar Projetos .NET 10
**Como** desenvolvedor  
**Quero** ter a solution e projetos configurados  
**Para** começar o desenvolvimento do backend

**Critérios de Aceitação:**
- [ ] Criar `KernelMind.sln` na raiz
- [ ] Criar projeto `KernelMind.Domain` (Class Library)
- [ ] Criar projeto `KernelMind.Infrastructure` (Class Library)
- [ ] Criar projeto `KernelMind.Core` (Class Library)
- [ ] Criar projeto `KernelMind.Api` (Web API)
- [ ] Configurar referências entre projetos
- [ ] Adicionar pacotes NuGet iniciais

**Dependências:** US-001
**Estimativa:** 1 hora
**Prioridade:** 🔴 Alta

---

### US-007: Implementar Entidades do Domínio
**Como** desenvolvedor  
**Quero** ter as entidades principais definidas  
**Para** representar os dados do sistema

**Critérios de Aceitação:**
- [ ] Criar `Pizza.cs` como `record` com: Id, Name, Description, Ingredients, Price, Category, Embedding
- [ ] Criar `Order.cs` como `record` com: Id, CustomerName, Phone, Address, Items, Total, Status
- [ ] Criar `OrderItem.cs` como `record` com: PizzaId, PizzaName, Quantity, UnitPrice, Notes
- [ ] Criar `Conversation.cs` como `record` com: Id, SessionId, Role, Content, Timestamp
- [ ] Todas as entidades devem ter Guid como chave primária
- [ ] Configurar data annotations ou fluent validation

**Dependências:** US-006
**Estimativa:** 2 horas
**Prioridade:** 🔴 Alta

---

### US-008: Configurar Entity Framework Core
**Como** desenvolvedor  
**Quero** ter o DbContext configurado  
**Para** acessar o banco de dados PostgreSQL

**Critérios de Aceitação:**
- [ ] Criar `AppDbContext.cs` em Infrastructure
- [ ] Configurar DbSets para todas as entidades
- [ ] Configurar string de conexão via appsettings.json
- [ ] Adicionar pacotes: Npgsql.EntityFrameworkCore.PostgreSQL, Pgvector.EntityFrameworkCore
- [ ] Configurar suporte a vetores (pgvector) no DbContext
- [ ] Configurar logging do EF Core

**Dependências:** US-007
**Estimativa:** 2 horas
**Prioridade:** 🔴 Alta

---

### US-009: Criar Primeiras Migrations
**Como** desenvolvedor  
**Quero** ter o schema do banco versionado  
**Para** criar as tabelas inicialmente

**Critérios de Aceitação:**
- [ ] Criar migration `InitialCreate`
- [ ] Migration deve criar tabelas: pizzas, orders, order_items, conversations
- [ ] Configurar índice vetorial para tabela Pizzas
- [ ] Aplicar migration com `dotnet ef database update`
- [ ] Verificar se tabelas foram criadas corretamente

**Dependências:** US-008
**Estimativa:** 1 hora
**Prioridade:** 🔴 Alta

---

### US-010: Implementar Repositórios
**Como** desenvolvedor  
**Quero** ter a camada de acesso a dados  
**Para** abstrair as operações CRUD

**Critérios de Aceitação:**
- [ ] Criar interface `IPizzaRepository` e implementação
- [ ] Criar interface `IOrderRepository` e implementação
- [ ] Criar interface `IConversationRepository` e implementação
- [ ] Implementar métodos básicos: GetAll, GetById, Add, Update, Delete
- [ ] Implementar métodos específicos: GetByName (Pizza), GetBySessao (Conversa)
- [ ] Injetar DbContext nos repositórios
- [ ] Configurar injeção de dependência no Program.cs

**Dependências:** US-009
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-011: Criar Seed Data do Cardápio
**Como** desenvolvedor  
**Quero** ter dados iniciais de pizzas  
**Para** popular o banco para testes

**Critérios de Aceitação:**
- [ ] Criar classe `SeedData.cs` com 15+ pizzas
- [ ] Incluir pizzas de diferentes categorias: Tradicional, Especial, Doce
- [ ] Criar comando CLI: `dotnet run --seed`
- [ ] Seed deve verificar duplicatas antes de inserir
- [ ] Documentar pizzas no README
- [ ] Testar seed e verificar se pizzas foram inseridas

**Dependências:** US-010
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

## 📦 FASE 2: Semantic Kernel e Plugins

### US-012: Configurar Semantic Kernel
**Como** desenvolvedor  
**Quero** ter o Semantic Kernel configurado  
**Para** integrar com o Ollama local

**Critérios de Aceitação:**
- [ ] Instalar pacotes: Microsoft.SemanticKernel, Microsoft.SemanticKernel.Connectors.Ollama
- [ ] Criar `KernelConfig.cs` com configuração do Kernel
- [ ] Configurar ChatCompletion com modelo llama3.1:70b
- [ ] Configurar TextEmbeddingGeneration com nomic-embed-text
- [ ] Configurar Ollama URL via appsettings
- [ ] Testar conexão com Ollama (health check)
- [ ] Criar serviço injetável: `IKernelService`

**Dependências:** US-006
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-013: Implementar MenuPlugin
**Como** usuário  
**Quero** consultar o cardápio via chat  
**Para** ver as pizzas disponíveis

**Critérios de Aceitação:**
- [ ] Criar `MenuPlugin.cs` com atributo `[KernelFunction]`
- [ ] Implementar função `list_menu`: lista todas as pizzas
- [ ] Implementar função `search_pizza`: busca por nome ou ingrediente
- [ ] Implementar função `get_pizza_details`: mostra detalhes de uma pizza
- [ ] Funções devem usar IPizzaRepository
- [ ] Formatar resposta de forma amigável (com emojis)
- [ ] Testar plugin isoladamente

**Dependências:** US-010, US-012
**Estimativa:** 4 horas
**Prioridade:** 🔴 Alta

---

### US-014: Implementar PedidoPlugin
**Como** usuário  
**Quero** adicionar pizzas ao pedido  
**Para** montar meu pedido via chat

**Critérios de Aceitação:**
- [ ] Criar `OrderPlugin.cs`
- [ ] Implementar função `add_item`: adiciona pizza ao pedido atual
- [ ] Implementar função `remove_item`: remove pizza do pedido
- [ ] Implementar função `view_order`: mostra pedido atual
- [ ] Implementar função `confirm_order`: salva pedido no banco
- [ ] Implementar função `cancel_order`: limpa pedido atual
- [ ] Manter estado do pedido em memória (sessão)
- [ ] Validar se pizza existe antes de adicionar
- [ ] Testar ciclo completo: adicionar → ver → confirmar

**Dependências:** US-013
**Estimativa:** 5 horas
**Prioridade:** 🔴 Alta

---

### US-015: Implementar CalculoPlugin
**Como** usuário  
**Quero** calcular valores e ver promoções  
**Para** saber o preço do pedido

**Critérios de Aceitação:**
- [ ] Criar `CalculationPlugin.cs`
- [ ] Implementar função `calculate_total`: soma valores dos itens
- [ ] Implementar função `apply_discount`: aplica % de desconto
- [ ] Implementar função `calculate_delivery_fee`: retorna taxa por bairro
- [ ] Implementar função `check_promotion`: mostra promoção do dia
- [ ] Criar dicionário de taxas por bairro
- [ ] Criar lógica de promoções por dia da semana
- [ ] Testar cálculos com diferentes cenários

**Dependências:** US-014
**Estimativa:** 3 horas
**Prioridade:** 🟡 Média

---

### US-016: Implementar ContextoPlugin
**Como** usuário  
**Quero** que o bot lembre da conversa  
**Para** ter contexto nas respostas

**Critérios de Aceitação:**
- [ ] Criar `ContextPlugin.cs`
- [ ] Implementar função `save_message`: salva no banco
- [ ] Implementar função `get_history`: busca últimas N mensagens
- [ ] Implementar função `clear_context`: remove histórico
- [ ] Usar IConversaRepository
- [ ] Limitar histórico às últimas 10 mensagens
- [ ] Formatar histórico para prompt do LLM

**Dependências:** US-010
**Estimativa:** 3 horas
**Prioridade:** 🟡 Média

---

### US-017: Criar ChatService
**Como** desenvolvedor  
**Quero** ter um serviço de chat unificado  
**Para** orquestrar plugins e LLM

**Critérios de Aceitação:**
- [ ] Criar `ChatService.cs`
- [ ] Injetar Kernel e todos os plugins
- [ ] Criar método `ProcessMessageAsync`: processa uma mensagem
- [ ] Criar método `StreamChatAsync`: processa com streaming (IAsyncEnumerable)
- [ ] Implementar pipeline: histórico → LLM → resposta
- [ ] Configurar prompt system para atendente de pizzaria
- [ ] Testar integração com todos os plugins

**Dependências:** US-013, US-014, US-015, US-016
**Estimativa:** 4 horas
**Prioridade:** 🔴 Alta

---

## 📦 FASE 3: RAG e Embeddings

### US-018: Configurar pgvector no Banco
**Como** desenvolvedor  
**Quero** ter suporte a vetores no PostgreSQL  
**Para** armazenar embeddings das pizzas

**Critérios de Aceitação:**
- [ ] Verificar se extensão pgvector está instalada
- [ ] Criar migration para adicionar coluna `embedding` (tipo vector)
- [ ] Criar índice ivfflat para busca vetorial
- [ ] Testar inserção de vetores manualmente
- [ ] Documentar dimensões do vetor (1536 para nomic-embed-text)

**Dependências:** US-009
**Estimativa:** 1 hora
**Prioridade:** 🔴 Alta

---

### US-019: Implementar EmbeddingService
**Como** desenvolvedor  
**Quero** gerar embeddings de textos  
**Para** vetorizar pizzas e consultas

**Critérios de Aceitação:**
- [ ] Criar `EmbeddingService.cs`
- [ ] Injetar ITextEmbeddingGenerationService
- [ ] Implementar `GenerateEmbeddingAsync(Pizza)`: gera vetor da pizza
- [ ] Implementar `GenerateQueryEmbeddingAsync(string)`: gera vetor da consulta
- [ ] Formatar texto da pizza: nome + descrição + ingredientes
- [ ] Retornar tipo Vector do pgvector
- [ ] Testar geração de embeddings

**Dependências:** US-012, US-018
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-020: Implementar VectorSearchService
**Como** usuário  
**Quero** buscar pizzas por similaridade  
**Para** encontrar pizzas por descrição

**Critérios de Aceitação:**
- [ ] Criar `VectorSearchService.cs`
- [ ] Injetar AppDbContext e EmbeddingService
- [ ] Implementar `SearchAsync(string query, int topK)`: busca semântica
- [ ] Usar cosine similarity do pgvector
- [ ] Ordenar por distância vetorial (mais similar primeiro)
- [ ] Retornar top-K resultados (padrão: 3)
- [ ] Testar busca: "pizza com bacon" deve retornar pizzas com bacon
- [ ] Testar busca: "doce" deve retornar pizzas doces

**Dependências:** US-019
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-021: Criar Pipeline de Vetorização do Cardápio
**Como** desenvolvedor  
**Quero** vetorizar todas as pizzas  
**Para** habilitar busca semântica

**Critérios de Aceitação:**
- [ ] Criar comando: `dotnet run --vectorize`
- [ ] Implementar `IndexAllPizzasAsync` no EmbeddingService
- [ ] Buscar todas as pizzas sem embedding
- [ ] Gerar embedding para cada uma
- [ ] Salvar embedding no banco
- [ ] Mostrar progresso no console
- [ ] Verificar se todas as pizzas foram vetorizadas

**Dependências:** US-020
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

### US-022: Integrar RAG no MenuPlugin
**Como** usuário  
**Quero** buscar pizzas semanticamente  
**Para** encontrar opções por descrição

**Critérios de Aceitação:**
- [ ] Atualizar `MenuPlugin.buscar_pizza` para usar VectorSearchService
- [ ] Quando termo não encontra match exato, usar busca vetorial
- [ ] Retornar top 3 resultados mais relevantes
- [ ] Formatar resposta com score de similaridade (opcional)
- [ ] Testar: "algo picante" → deve retornar pepperoni, 4 queijos
- [ ] Testar: "leve" → deve retornar margherita

**Dependências:** US-021
**Estimativa:** 2 horas
**Prioridade:** 🔴 Alta

---

## 📦 FASE 4: API REST com Streaming

### US-023: Criar Controllers da API
**Como** desenvolvedor  
**Quero** expor endpoints REST  
**Para** comunicação com frontend

**Critérios de Aceitação:**
- [ ] Criar `ChatController` com endpoint POST /api/chat
- [ ] Criar `MenuController` com endpoints: GET /api/menu, GET /api/menu/buscar
- [ ] Criar `PedidoController` com endpoints: POST /api/pedidos, GET /api/pedidos/{id}
- [ ] Configurar routing e atributos
- [ ] Configurar Swagger/OpenAPI
- [ ] Adicionar tratamento de erros global
- [ ] Configurar CORS para frontend Angular

**Dependências:** US-017, US-022
**Estimativa:** 4 horas
**Prioridade:** 🔴 Alta

---

### US-024: Implementar Endpoint de Chat com Streaming
**Como** usuário  
**Quero** ver a resposta sendo digitada  
**Para** ter experiência de chat em tempo real

**Critérios de Aceitação:**
- [ ] Criar endpoint POST /api/chat/stream
- [ ] Retornar `IAsyncEnumerable<string>`
- [ ] Usar `StreamChatAsync` do ChatService
- [ ] Configurar Content-Type: text/event-stream ou application/json+stream
- [ ] Usar `yield return` para cada token
- [ ] Forçar flush com `await Task.Yield()`
- [ ] Configurar CancellationToken para cancelar stream
- [ ] Testar com curl: `curl -N -X POST http://localhost:5076/api/chat/stream`

**Dependências:** US-023
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-025: Criar DTOs e Validações
**Como** desenvolvedor  
**Quero** ter contratos de API bem definidos  
**Para** tipagem no frontend

**Critérios de Aceitação:**
- [ ] Criar `MessageRequest` como `record` (message, sessionId)
- [ ] Criar `MessageResponse` como `record` (content, timestamp)
- [ ] Criar `PizzaDto` como `record` (id, name, description, price, ingredients)
- [ ] Criar `OrderRequest` como `record` (customerName, phone, address, items)
- [ ] Criar `OrderResponse` como `record` (id, number, total, status)
- [ ] Adicionar validações com FluentValidation ou DataAnnotations
- [ ] Retornar erros 400 com detalhes de validação

**Dependências:** US-023
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

### US-026: Documentar API com Swagger
**Como** desenvolvedor  
**Quero** ter documentação interativa da API  
**Para** facilitar testes e integração

**Critérios de Aceitação:**
- [ ] Configurar Swagger UI em /swagger
- [ ] Documentar todos os endpoints
- [ ] Adicionar exemplos de request/response
- [ ] Documentar códigos de erro possíveis
- [ ] Adicionar descrições em português
- [ ] Configurar Swagger para ambiente de desenvolvimento
- [ ] Testar todos os endpoints via Swagger UI

**Dependências:** US-025
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

## 📦 FASE 5: Frontend Angular

### US-027: Criar Projeto Angular 19
**Como** desenvolvedor  
**Quero** ter o projeto frontend configurado  
**Para** desenvolver a interface

**Critérios de Aceitação:**
- [ ] Criar projeto com `ng new KernelMind.Web --routing --style=scss`
- [ ] Configurar strict mode
- [ ] Instalar Angular Material: `ng add @angular/material`
- [ ] Configurar tema escuro/claro
- [ ] Estruturar pastas: components, services, models
- [ ] Configurar environments (dev/prod)
- [ ] Criar proxy.conf.json para desenvolvimento

**Dependências:** Nenhuma (pode ser feito em paralelo)
**Estimativa:** 2 horas
**Prioridade:** 🔴 Alta

---

### US-028: Criar Models TypeScript
**Como** desenvolvedor  
**Quero** ter as interfaces de dados  
**Para** tipagem forte no frontend

**Critérios de Aceitação:**
- [ ] Criar `pizza.model.ts`: interface Pizza (TypeScript não tem record, usar readonly quando possível)
- [ ] Criar `order.model.ts`: interfaces Order, OrderItem
- [ ] Criar `message.model.ts`: interface ChatMessage
- [ ] Todos os campos devem ser tipados
- [ ] Adicionar enums para StatusPedido, CategoriaPizza

**Dependências:** US-027
**Estimativa:** 1 hora
**Prioridade:** 🟡 Média

---

### US-029: Implementar ChatService
**Como** desenvolvedor  
**Quero** comunicar com backend via HTTP  
**Para** enviar mensagens e receber respostas

**Critérios de Aceitação:**
- [ ] Criar `ChatService` injetável
- [ ] Implementar `enviarMensagem(mensagem)`: POST simples
- [ ] Implementar `obterCardapio()`: GET /api/menu
- [ ] Implementar `buscarPizzas(termo)`: GET /api/menu/buscar
- [ ] Configurar base URL via environment
- [ ] Tratar erros HTTP com mensagens amigáveis

**Dependências:** US-028
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-030: Implementar StreamingService
**Como** usuário  
**Quero** ver a resposta sendo construída  
**Para** ter experiência fluida de chat

**Critérios de Aceitação:**
- [ ] Criar `StreamingService`
- [ ] Implementar `enviarMensagemStream(mensagem, onChunk)`
- [ ] Usar Fetch API nativa (não Angular HttpClient)
- [ ] Ler stream com `response.body.getReader()`
- [ ] Decodificar chunks com `TextDecoder`
- [ ] Chamar callback `onChunk` para cada pedaço recebido
- [ ] Tratamento de erros de conexão
- [ ] Suporte a cancelamento (AbortController)

**Dependências:** US-029
**Estimativa:** 4 horas
**Prioridade:** 🔴 Alta

---

### US-031: Implementar ChatComponent
**Como** usuário  
**Quero** uma interface de chat intuitiva  
**Para** conversar com o bot

**Critérios de Aceitação:**
- [ ] Criar `ChatComponent` com selector `app-chat`
- [ ] Layout: header, message area, input
- [ ] Mensagens com diferenciação visual (user vs bot)
- [ ] Suporte a markdown/html nas mensagens do bot
- [ ] Scroll automático para última mensagem
- [ ] Indicador de "typing..."
- [ ] Input com Enter para enviar
- [ ] Botão enviar desabilitado quando vazio
- [ ] Usar Angular Material (MatInput, MatButton, MatCard)

**Dependências:** US-030
**Estimativa:** 5 horas
**Prioridade:** 🔴 Alta

---

### US-032: Criar Componente de Pedido
**Como** usuário  
**Quero** ver meu pedido atual  
**Para** acompanhar o que estou comprando

**Critérios de Aceitação:**
- [ ] Criar `OrderComponent`
- [ ] Mostrar lista de itens com quantidade e preço
- [ ] Mostrar total do pedido
- [ ] Botão para remover item
- [ ] Botão para confirmar pedido (abre diálogo)
- [ ] Botão para cancelar pedido
- [ ] Atualizar em tempo real quando itens são adicionados

**Dependências:** US-031
**Estimativa:** 4 horas
**Prioridade:** 🟡 Média

---

### US-033: Implementar Tema Visual
**Como** usuário  
**Quero** uma interface bonita  
**Para** melhor experiência

**Critérios de Aceitação:**
- [ ] Configurar tema com cores da pizzaria (vermelho, amarelo)
- [ ] Criar variáveis SCSS para cores
- [ ] Estilizar ChatComponent (bubbles, avatares)
- [ ] Adicionar animações (fade in das mensagens)
- [ ] Tornar responsivo (mobile-friendly)
- [ ] Adicionar favicon e título da página
- [ ] Adicionar emojis relacionados a pizza 🍕

**Dependências:** US-031
**Estimativa:** 4 horas
**Prioridade:** 🟡 Média

---

### US-034: Configurar Proxy e Ambientes
**Como** desenvolvedor  
**Quero** comunicar frontend com backend  
**Para** desenvolvimento sem CORS

**Critérios de Aceitação:**
- [ ] Criar `proxy.conf.json` para redirecionar /api para localhost:5076
- [ ] Configurar angular.json para usar proxy
- [ ] Criar environments com URLs da API
- [ ] Configurar produção para usar nginx proxy
- [ ] Testar comunicação end-to-end
- [ ] Verificar se streaming funciona via proxy

**Dependências:** US-030
**Estimativa:** 2 horas
**Prioridade:** 🟡 Média

---

## 📦 FASE 6: Integração e Deploy

### US-035: Configurar Docker Compose Completo
**Como** desenvolvedor  
**Quero** subir toda a aplicação  
**Para** testar integração

**Critérios de Aceitação:**
- [ ] Atualizar docker-compose.yml com builds dos projetos
- [ ] Configurar depends_on entre serviços
- [ ] Testar `docker-compose up --build`
- [ ] Verificar se todos os containers iniciam
- [ ] Testar comunicação entre containers
- [ ] Verificar se frontend acessa backend via nginx

**Dependências:** Todas as anteriores
**Estimativa:** 3 horas
**Prioridade:** 🔴 Alta

---

### US-036: Testar Fluxo End-to-End
**Como** QA/Tester  
**Quero** validar o sistema completo  
**Para** garantir que tudo funciona

**Critérios de Aceitação:**
- [ ] Cenário 1: Usuário abre chat, vê saudação
- [ ] Cenário 2: Usuário pede cardápio, vê lista de pizzas
- [ ] Cenário 3: Usuário busca "bacon", vê resultados relevantes
- [ ] Cenário 4: Usuário adiciona pizzas ao pedido
- [ ] Cenário 5: Usuário confirma pedido, recebe número
- [ ] Cenário 6: Streaming funciona (resposta aparece gradativamente)
- [ ] Testar em diferentes navegadores (Chrome, Firefox)
- [ ] Testar em mobile (viewport reduzido)

**Dependências:** US-035
**Estimativa:** 4 horas
**Prioridade:** 🔴 Alta

---

### US-037: Otimizar Performance
**Como** desenvolvedor  
**Quero** melhorar velocidade  
**Para** melhor UX

**Critérios de Aceitação:**
- [ ] Verificar bundle size do Angular (lazy loading)
- [ ] Configurar caching do nginx
- [ ] Otimizar imagens (se houver)
- [ ] Verificar tempo de resposta do backend (< 2s para queries)
- [ ] Adicionar gzip compression
- [ ] Testar com Lighthouse (score > 80)

**Dependências:** US-036
**Estimativa:** 3 horas
**Prioridade:** 🟢 Baixa

---

### US-038: Criar Documentação Final
**Como** desenvolvedor  
**Quero** documentar o projeto  
**Para** outros desenvolvedores

**Critérios de Aceitação:**
- [ ] Atualizar README.md com instruções completas
- [ ] Criar diagrama de arquitetura
- [ ] Documentar como adicionar novas pizzas
- [ ] Documentar como treinar modelo RAG
- [ ] Criar vídeo demo (opcional)
- [ ] Documentar troubleshooting comum

**Dependências:** Todas
**Estimativa:** 4 horas
**Prioridade:** 🟢 Baixa

---

### US-039: Criar Testes Automatizados
**Como** desenvolvedor  
**Quero** ter testes automatizados  
**Para** garantir qualidade

**Critérios de Aceitação:**
- [ ] Testes unitários para Plugins (xUnit)
- [ ] Testes de integração para API
- [ ] Testes E2E para fluxo principal (Cypress ou Playwright)
- [ ] Cobertura mínima: 70%
- [ ] Pipeline de CI/CD (GitHub Actions)
- [ ] Rodar testes no `docker-compose` de testes

**Dependências:** Todas
**Estimativa:** 8 horas
**Prioridade:** 🟢 Baixa

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
- 🔴 **Alta**: 21 US (54%)
- 🟡 **Média**: 13 US (33%)
- 🟢 **Baixa**: 5 US (13%)

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
