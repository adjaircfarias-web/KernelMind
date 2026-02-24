# 🧠 KernelMind - Implementation Plan

**AI-Powered Pizza Ordering Chatbot**

*(This document is historical. For current architecture see [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md).)*

---

## 📋 Project Overview

### 🎯 Goal
Build an intelligent conversational chatbot for pizza orders, demonstrating **Semantic Kernel**, **RAG (Retrieval Augmented Generation)**, **Embeddings** and **Plugins (Tooling)** integrated with a local language model via Ollama.

### 🏷️ Metadata
- **Project Name**: KernelMind
- **Version**: 1.0.0
- **Status**: In planning
- **Start Date**: 2026-02-06
- **Stack**: .NET 10, C#, Semantic Kernel, PostgreSQL, Ollama

### 🎨 Concept
The user interacts with a chatbot that:
1. ✅ Understands orders in natural language
2. ✅ Queries the menu via RAG (semantic search)
3. ✅ Calculates prices and confirms orders via Plugins
4. ✅ Maintains conversation context
5. ✅ Processes everything locally (no external APIs)

---

## 🎯 Main Features

### 1. 🤖 Conversational Chatbot
- Simple chat interface (CLI or Web)
- Natural language processing
- Conversation context maintenance
- Responses in Portuguese

### 2. 📚 RAG + Embeddings
- Pizza menu vectorization
- Semantic search by ingredients/flavors
- Relevant information retrieval
- Contextualized response generation

### 3. 🔌 Plugins (Tooling)
- **MenuPlugin**: Query menu, ingredients, prices
- **OrderPlugin**: Add items, confirm order, cancel
- **CalculationPlugin**: Calculate total, apply discounts, delivery fee
- **ContextPlugin**: Maintain conversation history

### 4. 🗄️ Persistence
- PostgreSQL for structured data
- pgvector for embedding storage
- Conversation history
- Order data

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    KERNELMIND                               │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Interface do Usuário                    │  │
│  │         (Console App / Web API / Blazor)            │  │
│  └──────────────────────┬───────────────────────────────┘  │
│                         │                                   │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │            Semantic Kernel Core                      │  │
│  │  ┌─────────────┐  ┌──────────────┐  ┌───────────┐  │  │
│  │  │   Kernel    │  │   Plugins    │  │   RAG     │  │  │
│  │  │             │  │  • Menu      │  │  System   │  │  │
│  │  │  • Config   │  │  • Pedido    │  │           │  │  │
│  │  │  • Memory   │  │  • Calculo   │  │  • Embed  │  │  │
│  │  │  • Chat     │  │  • Contexto  │  │  • Search │  │  │
│  │  └─────────────┘  └──────────────┘  └───────────┘  │  │
│  └──────────────────────┬───────────────────────────────┘  │
│                         │                                   │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │              Ollama (Local LLM)                      │  │
│  │            Model: llama3.1:70b                       │  │
│  └──────────────────────┬───────────────────────────────┘  │
│                         │                                   │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │              PostgreSQL + pgvector                   │  │
│  │  ┌─────────┐  ┌──────────┐  ┌──────────┐            │  │
│  │  │  Pizzas │  │ Pedidos  │  │Embeddings│            │  │
│  │  └─────────┘  └──────────┘  └──────────┘            │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 Project Structure

```
KernelMind/
├── 📁 src/
│   ├── 📁 KernelMind.Api/              # API REST (opcional)
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── 📁 KernelMind.Cli/              # Interface Console
│   │   ├── Program.cs
│   │   └── Services/
│   │
│   ├── 📁 KernelMind.Core/             # Núcleo do Sistema
│   │   ├── 📁 Plugins/
│   │   │   ├── MenuPlugin.cs
│   │   │   ├── PedidoPlugin.cs
│   │   │   ├── CalculoPlugin.cs
│   │   │   └── ContextoPlugin.cs
│   │   │
│   │   ├── 📁 Services/
│   │   │   ├── ChatService.cs
│   │   │   ├── EmbeddingService.cs
│   │   │   └── VectorSearchService.cs
│   │   │
│   │   ├── 📁 Configuration/
│   │   │   ├── KernelConfig.cs
│   │   │   └── OllamaConfig.cs
│   │   │
│   │   └── KernelMind.Core.csproj
│   │
│   ├── 📁 KernelMind.Domain/           # Entidades
│   │   ├── Pizza.cs
│   │   ├── Pedido.cs
│   │   ├── ItemPedido.cs
│   │   ├── Cliente.cs
│   │   └── Conversa.cs
│   │
│   └── 📁 KernelMind.Infrastructure/   # Dados
│       ├── 📁 Data/
│       │   ├── AppDbContext.cs
│       │   └── Configurations/
│       │
│       ├── 📁 Repositories/
│       │   ├── PizzaRepository.cs
│       │   └── PedidoRepository.cs
│       │
│       └── Migrations/
│
├── 📁 docs/                            # Documentação
│   ├── API.md
│   └── EXEMPLOS.md
│
├── 📁 scripts/                         # Scripts utilitários
│   ├── setup-database.ps1
│   └── seed-data.ps1
│
├── 📁 tests/                           # Testes
│   └── KernelMind.Tests/
│
├── docker-compose.yml                  # PostgreSQL + pgvector
├── README.md
└── KernelMind.sln
```

---

## 🗄️ Modelo de Dados

### Entidades Principais

```csharp
// Pizza.cs - Cardápio
public class Pizza
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public List<string> Ingredientes { get; set; } = new();
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = string.Empty; // Tradicional, Especial, Doce
    public string? Embedding { get; set; } // Vetor serializado
}

// Pedido.cs - Pedido do cliente
public class Pedido
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public List<ItemPedido> Itens { get; set; } = new();
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTime DataCriacao { get; set; }
}

// ItemPedido.cs - Item do pedido
public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PizzaId { get; set; }
    public string PizzaNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string? Observacoes { get; set; }
}

// Conversa.cs - Histórico de chat
public class Conversa
{
    public Guid Id { get; set; }
    public string SessaoId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // user / assistant
    public string Conteudo { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
```

### Schema do Banco PostgreSQL

```sql
-- Tabela de Pizzas
CREATE TABLE pizzas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(100) NOT NULL,
    descricao TEXT,
    ingredientes TEXT[],
    preco DECIMAL(10,2) NOT NULL,
    categoria VARCHAR(50),
    embedding VECTOR(1536) -- Dimensão do modelo de embedding
);

-- Tabela de Pedidos
CREATE TABLE pedidos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome_cliente VARCHAR(100),
    telefone VARCHAR(20),
    endereco TEXT,
    total DECIMAL(10,2),
    status VARCHAR(50) DEFAULT 'Pendente',
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Itens do Pedido
CREATE TABLE itens_pedido (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id UUID REFERENCES pedidos(id),
    pizza_id UUID REFERENCES pizzas(id),
    pizza_nome VARCHAR(100),
    quantidade INTEGER,
    preco_unitario DECIMAL(10,2),
    observacoes TEXT
);

-- Tabela de Conversas
CREATE TABLE conversas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sessao_id VARCHAR(100),
    role VARCHAR(20),
    conteudo TEXT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índice para busca vetorial (RAG)
CREATE INDEX idx_pizzas_embedding ON pizzas 
USING ivfflat (embedding vector_cosine_ops);
```

---

## 🔌 Plugins (Tooling)

### 1. MenuPlugin - Consulta de Cardápio

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

public class MenuPlugin
{
    private readonly IPizzaRepository _pizzaRepo;
    private readonly IVectorSearchService _vectorSearch;

    public MenuPlugin(IPizzaRepository pizzaRepo, IVectorSearchService vectorSearch)
    {
        _pizzaRepo = pizzaRepo;
        _vectorSearch = vectorSearch;
    }

    [KernelFunction("listar_cardapio")]
    [Description("Lista todas as pizzas disponíveis no cardápio")]
    public async Task<string> ListarCardapioAsync()
    {
        var pizzas = await _pizzaRepo.GetAllAsync();
        var menu = string.Join("\n", pizzas.Select(p => 
            $"🍕 {p.Nome} - R$ {p.Preco:F2}\n   {p.Descricao}"));
        
        return $"Aqui está nosso cardápio:\n\n{menu}";
    }

    [KernelFunction("buscar_pizza")]
    [Description("Busca pizzas por nome ou ingrediente")]
    public async Task<string> BuscarPizzaAsync(
        [Description("Termo de busca: nome da pizza ou ingrediente")] string termo)
    {
        // Busca semântica usando embeddings
        var pizzas = await _vectorSearch.SearchAsync(termo, topK: 3);
        
        if (!pizzas.Any())
            return "Desculpe, não encontrei pizzas com esse termo.";
        
        var resultados = string.Join("\n\n", pizzas.Select(p =>
            $"🍕 {p.Nome} - R$ {p.Preco:F2}\n" +
            $"   Ingredientes: {string.Join(", ", p.Ingredientes)}"));
        
        return $"Encontrei essas opções:\n\n{resultados}";
    }

    [KernelFunction("detalhes_pizza")]
    [Description("Obtém detalhes de uma pizza específica")]
    public async Task<string> ObterDetalhesAsync(
        [Description("Nome da pizza")] string nomePizza)
    {
        var pizza = await _pizzaRepo.GetByNameAsync(nomePizza);
        
        if (pizza == null)
            return $"Não encontrei a pizza '{nomePizza}'.";
        
        return $"🍕 {pizza.Nome}\n" +
               $"💰 Preço: R$ {pizza.Preco:F2}\n" +
               $"📝 {pizza.Descricao}\n" +
               $"🥘 Ingredientes: {string.Join(", ", pizza.Ingredientes)}";
    }
}
```

### 2. PedidoPlugin - Gestão de Pedidos

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

public class PedidoPlugin
{
    private readonly IPedidoRepository _pedidoRepo;
    private readonly IPizzaRepository _pizzaRepo;
    private Pedido _pedidoAtual;

    public PedidoPlugin(IPedidoRepository pedidoRepo, IPizzaRepository pizzaRepo)
    {
        _pedidoRepo = pedidoRepo;
        _pizzaRepo = pizzaRepo;
        _pedidoAtual = new Pedido();
    }

    [KernelFunction("adicionar_item")]
    [Description("Adiciona uma pizza ao pedido atual")]
    public async Task<string> AdicionarItemAsync(
        [Description("Nome da pizza")] string nomePizza,
        [Description("Quantidade (padrão: 1)")] int quantidade = 1,
        [Description("Observações especiais")] string? observacoes = null)
    {
        var pizza = await _pizzaRepo.GetByNameAsync(nomePizza);
        
        if (pizza == null)
            return $"❌ Pizza '{nomePizza}' não encontrada no cardápio.";

        var item = new ItemPedido
        {
            PizzaId = pizza.Id,
            PizzaNome = pizza.Nome,
            Quantidade = quantidade,
            PrecoUnitario = pizza.Preco,
            Observacoes = observacoes
        };

        _pedidoAtual.Itens.Add(item);
        
        return $"✅ Adicionei {quantidade}x {pizza.Nome} ao seu pedido.\n" +
               $"💰 Subtotal: R$ {(_pedidoAtual.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)):F2}";
    }

    [KernelFunction("remover_item")]
    [Description("Remove uma pizza do pedido atual")]
    public Task<string> RemoverItemAsync(
        [Description("Nome da pizza a remover")] string nomePizza)
    {
        var item = _pedidoAtual.Itens.FirstOrDefault(i => 
            i.PizzaNome.Equals(nomePizza, StringComparison.OrdinalIgnoreCase));
        
        if (item == null)
            return Task.FromResult($"❌ '{nomePizza}' não está no seu pedido.");

        _pedidoAtual.Itens.Remove(item);
        
        return Task.FromResult($"✅ Removi {nomePizza} do seu pedido.");
    }

    [KernelFunction("ver_pedido")]
    [Description("Mostra o pedido atual")]
    public Task<string> VerPedidoAsync()
    {
        if (!_pedidoAtual.Itens.Any())
            return Task.FromResult("🛒 Seu pedido está vazio.");

        var itens = string.Join("\n", _pedidoAtual.Itens.Select(i =>
            $"• {i.Quantidade}x {i.PizzaNome} - R$ {(i.Quantidade * i.PrecoUnitario):F2}" +
            (i.Observacoes != null ? $"\n  Obs: {i.Observacoes}" : "")));

        var total = _pedidoAtual.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        return Task.FromResult($"🛒 Seu Pedido:\n\n{itens}\n\n" +
                               $"💰 Total: R$ {total:F2}");
    }

    [KernelFunction("confirmar_pedido")]
    [Description("Confirma e finaliza o pedido")]
    public async Task<string> ConfirmarPedidoAsync(
        [Description("Nome do cliente")] string nomeCliente,
        [Description("Telefone para contato")] string telefone,
        [Description("Endereço de entrega")] string endereco)
    {
        if (!_pedidoAtual.Itens.Any())
            return "❌ Não há itens no pedido para confirmar.";

        _pedidoAtual.NomeCliente = nomeCliente;
        _pedidoAtual.Telefone = telefone;
        _pedidoAtual.Endereco = endereco;
        _pedidoAtual.Total = _pedidoAtual.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        _pedidoAtual.Status = "Confirmado";

        await _pedidoRepo.SaveAsync(_pedidoAtual);

        var numeroPedido = _pedidoAtual.Id.ToString()[..8];
        
        return $"✅ Pedido Confirmado!\n\n" +
               $"📋 Número: #{numeroPedido}\n" +
               $"👤 Cliente: {nomeCliente}\n" +
               $"📞 Telefone: {telefone}\n" +
               $"📍 Endereço: {endereco}\n" +
               $"💰 Total: R$ {_pedidoAtual.Total:F2}\n\n" +
               $"⏱️ Tempo estimado: 45 minutos";
    }

    [KernelFunction("cancelar_pedido")]
    [Description("Cancela o pedido atual")]
    public Task<string> CancelarPedidoAsync()
    {
        _pedidoAtual = new Pedido();
        return Task.FromResult("🗑️ Pedido cancelado. Pode começar um novo!");
    }
}
```

### 3. CalculoPlugin - Cálculos e Promoções

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

public class CalculoPlugin
{
    [KernelFunction("calcular_total")]
    [Description("Calcula o valor total do pedido")]
    public Task<string> CalcularTotalAsync(
        [Description("Lista de itens no formato: quantidade x preço_unitário")] string itens)
    {
        // Parser simples de itens
        var total = 0m;
        var linhas = itens.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var linha in linhas)
        {
            var partes = linha.Split('x');
            if (partes.Length == 2 && 
                int.TryParse(partes[0].Trim(), out var qtd) &&
                decimal.TryParse(partes[1].Trim(), out var preco))
            {
                total += qtd * preco;
            }
        }

        return Task.FromResult($"💰 Total do pedido: R$ {total:F2}");
    }

    [KernelFunction("aplicar_desconto")]
    [Description("Aplica desconto no valor total")]
    public Task<string> AplicarDescontoAsync(
        [Description("Valor original")] decimal valor,
        [Description("Percentual de desconto (ex: 10 para 10%)")] decimal percentual)
    {
        var desconto = valor * (percentual / 100);
        var valorFinal = valor - desconto;

        return Task.FromResult($"💰 Valor original: R$ {valor:F2}\n" +
                               $"🎉 Desconto ({percentual}%): -R$ {desconto:F2}\n" +
                               $"✨ Valor final: R$ {valorFinal:F2}");
    }

    [KernelFunction("calcular_taxa_entrega")]
    [Description("Calcula taxa de entrega baseada na distância")]
    public Task<string> CalcularTaxaEntregaAsync(
        [Description("Bairro de entrega")] string bairro)
    {
        var taxas = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["centro"] = 5.00m,
            ["jardins"] = 7.00m,
            ["vila mariana"] = 8.00m,
            ["pinheiros"] = 9.00m,
            ["moema"] = 10.00m,
            ["brooklin"] = 12.00m,
            ["morumbi"] = 15.00m
        };

        if (taxas.TryGetValue(bairro, out var taxa))
        {
            return Task.FromResult($"🛵 Taxa de entrega para {bairro}: R$ {taxa:F2}");
        }

        return Task.FromResult($"🛵 Taxa de entrega para {bairro}: R$ 10.00 (padrão)");
    }

    [KernelFunction("verificar_promocao")]
    [Description("Verifica promoções ativas")]
    public Task<string> VerificarPromocaoAsync(
        [Description("Dia da semana (opcional)")] string? diaSemana = null)
    {
        diaSemana ??= DateTime.Now.DayOfWeek.ToString();
        
        var promocoes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["monday"] = "🎉 Segunda-feira: 20% OFF em pizzas grandes!",
            ["tuesday"] = "🎉 Terça-feira: Compre 1, Leve 2 na categoria Doce!",
            ["wednesday"] = "🎉 Quarta-feira: Frete grátis acima de R$ 50!",
            ["thursday"] = "🎉 Quinta-feira: 15% OFF em todas as pizzas!",
            ["friday"] = "🎉 Sexta-feira: Combo Família (2 grandes + refrigerante) por R$ 89,90!",
            ["saturday"] = "🎉 Sábado: Refrigerante 2L grátis em pedidos acima de R$ 60!",
            ["sunday"] = "🎉 Domingo: 25% OFF no segundo item!"
        };

        if (promocoes.TryGetValue(diaSemana, out var promocao))
        {
            return Task.FromResult(promocao);
        }

        return Task.FromResult("📅 Hoje não temos promoções especiais, mas nossos preços são ótimos!");
    }
}
```

### 4. ContextoPlugin - Memória da Conversa

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KernelMind.Core.Plugins;

public class ContextoPlugin
{
    private readonly IConversaRepository _conversaRepo;
    private readonly string _sessaoId;

    public ContextoPlugin(IConversaRepository conversaRepo, string sessaoId)
    {
        _conversaRepo = conversaRepo;
        _sessaoId = sessaoId;
    }

    [KernelFunction("salvar_mensagem")]
    [Description("Salva uma mensagem no histórico da conversa")]
    public async Task SalvarMensagemAsync(
        [Description("Quem enviou: 'user' ou 'assistant'")] string role,
        [Description("Conteúdo da mensagem")] string conteudo)
    {
        var conversa = new Conversa
        {
            SessaoId = _sessaoId,
            Role = role,
            Conteudo = conteudo,
            Timestamp = DateTime.UtcNow
        };

        await _conversaRepo.SaveAsync(conversa);
    }

    [KernelFunction("recuperar_historico")]
    [Description("Recupera o histórico recente da conversa")]
    public async Task<string> RecuperarHistoricoAsync(
        [Description("Número de mensagens a recuperar")] int limite = 10)
    {
        var mensagens = await _conversaRepo.GetBySessaoAsync(_sessaoId, limite);
        
        if (!mensagens.Any())
            return "Início da conversa.";

        var historico = string.Join("\n", mensagens.Select(m => 
            $"{m.Role}: {m.Conteudo}"));

        return historico;
    }

    [KernelFunction("limpar_contexto")]
    [Description("Limpa o histórico da conversa atual")]
    public async Task<string> LimparContextoAsync()
    {
        await _conversaRepo.DeleteBySessaoAsync(_sessaoId);
        return "🧠 Contexto da conversa limpo. Podemos começar de novo!";
    }
}
```

---

## 🤖 Fluxo RAG (Retrieval Augmented Generation)

### Como Funciona o RAG no KernelMind

```
1. Usuário faz pergunta
   ↓
2. Gerar embedding da pergunta
   ↓
3. Buscar no banco vetorial (pgvector)
   ↓
4. Recuperar top-K documentos relevantes
   ↓
5. Adicionar contexto ao prompt do LLM
   ↓
6. Gerar resposta contextualizada
```

### Implementação do Serviço de Embedding

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Pgvector;

namespace KernelMind.Core.Services;

public class EmbeddingService
{
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly AppDbContext _dbContext;

    public EmbeddingService(
        ITextEmbeddingGenerationService embeddingService,
        AppDbContext dbContext)
    {
        _embeddingService = embeddingService;
        _dbContext = dbContext;
    }

    // Gerar embedding para uma pizza
    public async Task<Vector> GenerateEmbeddingAsync(Pizza pizza)
    {
        var texto = $"{pizza.Nome}. {pizza.Descricao}. " +
                    $"Ingredientes: {string.Join(", ", pizza.Ingredientes)}. " +
                    $"Categoria: {pizza.Categoria}.";

        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(new[] { texto });
        return new Vector(embeddings.First().ToArray());
    }

    // Gerar embedding para uma consulta
    public async Task<Vector> GenerateQueryEmbeddingAsync(string query)
    {
        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(new[] { query });
        return new Vector(embeddings.First().ToArray());
    }

    // Indexar todas as pizzas
    public async Task IndexAllPizzasAsync()
    {
        var pizzas = await _dbContext.Pizzas.ToListAsync();
        
        foreach (var pizza in pizzas)
        {
            var embedding = await GenerateEmbeddingAsync(pizza);
            pizza.Embedding = embedding.ToString();
        }

        await _dbContext.SaveChangesAsync();
    }
}

// Serviço de busca vetorial
public class VectorSearchService
{
    private readonly AppDbContext _dbContext;
    private readonly EmbeddingService _embeddingService;

    public VectorSearchService(
        AppDbContext dbContext,
        EmbeddingService embeddingService)
    {
        _dbContext = dbContext;
        _embeddingService = embeddingService;
    }

    public async Task<List<Pizza>> SearchAsync(string query, int topK = 3)
    {
        // Gerar embedding da consulta
        var queryEmbedding = await _embeddingService.GenerateQueryEmbeddingAsync(query);

        // Buscar no PostgreSQL usando pgvector
        var results = await _dbContext.Pizzas
            .OrderBy(p => p.Embedding!.VectorDistance(queryEmbedding, distanceFunction: DistanceFunction.Cosine))
            .Take(topK)
            .ToListAsync();

        return results;
    }
}
```

### Configuração do RAG no Semantic Kernel

```csharp
// KernelConfig.cs
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Embeddings;
using KernelMind.Core.Plugins;
using KernelMind.Core.Services;

namespace KernelMind.Core.Configuration;

public static class KernelConfig
{
    public static IKernelBuilder AddKernelMindServices(
        this IKernelBuilder builder,
        string ollamaUrl = "http://localhost:11434",
        string model = "llama3.1:70b")
    {
        // Configurar Ollama Chat Completion
        builder.AddOllamaChatCompletion(
            modelId: model,
            endpoint: new Uri(ollamaUrl));

        // Configurar Ollama Embedding (se disponível)
        // Ou usar modelo de embedding local
        builder.AddOllamaTextEmbeddingGeneration(
            modelId: "nomic-embed-text", // Modelo leve para embeddings
            endpoint: new Uri(ollamaUrl));

        // Registrar plugins
        builder.Plugins.AddFromType<MenuPlugin>();
        builder.Plugins.AddFromType<PedidoPlugin>();
        builder.Plugins.AddFromType<CalculoPlugin>();
        builder.Plugins.AddFromType<ContextoPlugin>();

        // Registrar serviços
        builder.Services.AddScoped<EmbeddingService>();
        builder.Services.AddScoped<VectorSearchService>();
        builder.Services.AddScoped<ChatService>();

        return builder;
    }
}
```

---

## 📋 Fases de Implementação

### 🎯 FASE 1: Setup e Infraestrutura (Dias 1-2)

**Objetivo**: Preparar ambiente de desenvolvimento

#### Tarefas:
- [ ] Criar estrutura de pastas do projeto
- [ ] Configurar Docker Compose (PostgreSQL + pgvector)
- [ ] Criar solution e projetos .NET 10
- [ ] Configurar Entity Framework Core
- [ ] Criar primeiras migrations
- [ ] Configurar Ollama local
- [ ] Testar conexão com LLM

**Artefatos:**
```
✅ docker-compose.yml
✅ KernelMind.sln
✅ Projetos criados
✅ Banco configurado
```

---

### 🎯 FASE 2: Domínio e Dados (Dias 3-4)

**Objetivo**: Implementar entidades e repositórios

#### Tarefas:
- [ ] Criar entidades (Pizza, Pedido, ItemPedido, Cliente, Conversa)
- [ ] Configurar DbContext
- [ ] Criar migrations do banco
- [ ] Implementar repositórios
- [ ] Criar seed data (cardápio inicial)
- [ ] Testar CRUD básico

**Artefatos:**
```
✅ Entidades do domínio
✅ Repositories
✅ Seed data (10-15 pizzas)
✅ Testes de integração
```

**Exemplo de Seed Data:**
```csharp
public static class SeedData
{
    public static List<Pizza> GetPizzas() => new()
    {
        new Pizza
        {
            Id = Guid.NewGuid(),
            Nome = "Margherita",
            Descricao = "Clássica italiana com molho de tomate, mussarela e manjericão fresco",
            Ingredientes = new() { "Molho de Tomate", "Mussarela", "Manjericão", "Azeite" },
            Preco = 45.90m,
            Categoria = "Tradicional"
        },
        new Pizza
        {
            Id = Guid.NewGuid(),
            Nome = "Pepperoni",
            Descricao = "Mussarela, pepperoni importado e orégano",
            Ingredientes = new() { "Molho de Tomate", "Mussarela", "Pepperoni", "Orégano" },
            Preco = 52.90m,
            Categoria = "Tradicional"
        },
        // ... mais pizzas
    };
}
```

---

### 🎯 FASE 3: Core e Plugins (Dias 5-8)

**Objetivo**: Implementar Semantic Kernel e plugins

#### Tarefas:
- [ ] Configurar Semantic Kernel
- [ ] Implementar MenuPlugin
- [ ] Implementar PedidoPlugin
- [ ] Implementar CalculoPlugin
- [ ] Implementar ContextoPlugin
- [ ] Testar plugins isoladamente
- [ ] Criar serviço de chat

**Artefatos:**
```
✅ Kernel configurado
✅ 4 Plugins implementados
✅ Serviço de chat
✅ Testes unitários dos plugins
```

---

### 🎯 FASE 4: RAG e Embeddings (Dias 9-11)

**Objetivo**: Implementar busca semântica

#### Tarefas:
- [ ] Configurar serviço de embeddings
- [ ] Criar pipeline de vetorização do cardápio
- [ ] Implementar VectorSearchService
- [ ] Integrar RAG no fluxo de chat
- [ ] Testar buscas semânticas
- [ ] Otimizar performance

**Artefatos:**
```
✅ EmbeddingService
✅ VectorSearchService
✅ Cardápio vetorizado
✅ Busca semântica funcionando
```

**Exemplo de teste RAG:**
```csharp
// Teste: Buscar pizza com "queijo" deve encontrar várias opções
[Fact]
public async Task Deve_Buscar_Pizzas_Com_Ingrediente_Queijo()
{
    var resultado = await _vectorSearch.SearchAsync("pizza com queijo", topK: 5);
    
    Assert.True(resultado.Count > 0);
    Assert.All(resultado, pizza => 
        Assert.Contains(pizza.Ingredientes, i => i.Contains("Mussarela")));
}
```

---

### 🎯 FASE 5: Interface CLI (Dias 12-13)

**Objetivo**: Criar interface console interativa

#### Tarefas:
- [ ] Criar projeto Console
- [ ] Implementar loop de conversa
- [ ] Adicionar formatação visual (cores, emojis)
- [ ] Implementar comandos especiais (/menu, /pedido, /sair)
- [ ] Testar fluxo completo

**Artefatos:**
```
✅ CLI interativo
✅ Interface amigável
✅ Comandos implementados
```

**Exemplo de CLI:**
```csharp
public class ChatInterface
{
    public async Task RunAsync()
    {
        Console.WriteLine("🧠 KernelMind - Chatbot de Pizzas");
        Console.WriteLine("Digite 'sair' para encerrar\n");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Você: ");
            Console.ResetColor();
            
            var input = Console.ReadLine();
            
            if (input?.ToLower() == "sair") break;

            var resposta = await _chatService.SendMessageAsync(input);
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"🤖 Bot: {resposta}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
```

---

### 🎯 FASE 6: Testes e Documentação (Dias 14-15)

**Objetivo**: Garantir qualidade e documentar

#### Tarefas:
- [ ] Escrever testes unitários (cobertura > 70%)
- [ ] Escrever testes de integração
- [ ] Criar documentação de API
- [ ] Escrever README completo
- [ ] Criar guia de instalação
- [ ] Testar end-to-end

**Artefatos:**
```
✅ Suite de testes
✅ Documentação completa
✅ README.md
✅ Guia de instalação
```

---

## 🛠️ Stack Tecnológico

### Framework e Runtime
| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| .NET | 10.0 | Framework principal |
| C# | 13.0 | Linguagem de programação |

### Semantic Kernel
| Pacote | Versão | Função |
|--------|--------|--------|
| Microsoft.SemanticKernel | 1.30+ | Core do Semantic Kernel |
| Microsoft.SemanticKernel.Connectors.Ollama | 1.30+ | Conector Ollama |
| Microsoft.SemanticKernel.Plugins.Core | 1.30+ | Plugins base |

### Banco de Dados
| Pacote | Versão | Função |
|--------|--------|--------|
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0+ | Driver PostgreSQL |
| Pgvector.EntityFrameworkCore | 0.2+ | Suporte a vetores |
| Microsoft.EntityFrameworkCore | 9.0+ | ORM |

### Utilitários
| Pacote | Versão | Função |
|--------|--------|--------|
| Spectre.Console | 0.49+ | Interface CLI bonita |
| Microsoft.Extensions.Configuration | 9.0+ | Configuração |
| Serilog | 4.0+ | Logging |

### Testes
| Pacote | Versão | Função |
|--------|--------|--------|
| xUnit | 2.9+ | Framework de testes |
| Moq | 4.20+ | Mocking |
| FluentAssertions | 6.12+ | Asserções |

---

## 🚀 Guia de Instalação

### Pré-requisitos

1. **.NET 10 SDK**
   ```bash
   # Verificar instalação
   dotnet --version
   # Deve mostrar: 10.0.xxx
   ```

2. **Docker Desktop**
   - [Download](https://www.docker.com/products/docker-desktop/)
   - Verificar: `docker --version`

3. **Ollama**
   - [Download](https://ollama.com)
   - Verificar: `ollama --version`

### Passo a Passo

#### 1. Clone o Repositório
```bash
git clone <url-do-repo>
cd KernelMind
```

#### 2. Inicie o Banco de Dados
```bash
docker-compose up -d

# Verificar se está rodando
docker-compose ps
```

#### 3. Baixe o Modelo Ollama
```bash
# Baixar modelo principal (70B)
ollama pull llama3.1:70b

# Baixar modelo para embeddings
ollama pull nomic-embed-text

# Verificar modelos
ollama list
```

#### 4. Configure o App
```bash
cd src/KernelMind.Cli

# Copiar configuração de exemplo
cp appsettings.example.json appsettings.json

# Editar appsettings.json se necessário
```

**appsettings.json:**
```json
{
  "Ollama": {
    "Url": "http://localhost:11434",
    "ChatModel": "llama3.1:70b",
    "EmbeddingModel": "nomic-embed-text"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=kernelmind;Username=postgres;Password=postgres123"
  }
}
```

#### 5. Execute as Migrations
```bash
cd src/KernelMind.Infrastructure
dotnet ef database update --startup-project ../KernelMind.Cli
```

#### 6. Popule o Cardápio
```bash
cd src/KernelMind.Cli
dotnet run --seed
```

#### 7. Execute a Aplicação
```bash
dotnet run
```

**Pronto!** 🎉 O chatbot está rodando!

---

## 📚 Exemplos de Uso

### Cenário 1: Primeiro Contato
```
🤖: Olá! Bem-vindo à Pizzaria KernelMind! 🍕
    Como posso ajudar você hoje?

Você: Quero ver o cardápio

🤖: Aqui está nosso cardápio:

🍕 Margherita - R$ 45,90
   Clássica italiana com molho de tomate, mussarela e manjericão fresco

🍕 Pepperoni - R$ 52,90
   Mussarela, pepperoni importado e orégano

[... mais pizzas]
```

### Cenário 2: Busca Inteligente (RAG)
```
Você: Quero uma pizza com bacon

🤖: Encontrei essas opções:

🍕 Bacon Supreme - R$ 58,90
   Ingredientes: Molho de Tomate, Mussarela, Bacon, Cebola, Azeitonas

🍕 Carbonara - R$ 56,90
   Ingredientes: Creme de Leite, Mussarela, Bacon, Parmesão, Ovos
```

### Cenário 3: Fazendo Pedido
```
Você: Quero 2 Margheritas

🤖: ✅ Adicionei 2x Margherita ao seu pedido.
    💰 Subtotal: R$ 91,80

Você: E 1 Pepperoni grande

🤖: ✅ Adicionei 1x Pepperoni ao seu pedido.
    💰 Subtotal: R$ 144,70

Você: Ver meu pedido

🤖: 🛒 Seu Pedido:

    • 2x Margherita - R$ 91,80
    • 1x Pepperoni - R$ 52,90

    💰 Total: R$ 144,70

Você: Confirmar pedido

🤖: Para confirmar, preciso de alguns dados:
    Qual seu nome?

Você: João Silva

🤖: Telefone para contato?

Você: 11999998888

🤖: Endereço de entrega?

Você: Rua das Pizzas, 123

🤖: ✅ Pedido Confirmado!

    📋 Número: #a7f3b2d1
    👤 Cliente: João Silva
    📞 Telefone: 11999998888
    📍 Endereço: Rua das Pizzas, 123
    💰 Total: R$ 144,70

    ⏱️ Tempo estimado: 45 minutos
```

---

## 🔮 Funcionalidades Futuras

### Versão 1.1
- [ ] API REST completa
- [ ] Interface Web (Blazor)
- [ ] Autenticação de usuários
- [ ] Histórico de pedidos
- [ ] Avaliações de pizzas

### Versão 1.2
- [ ] Integração com WhatsApp
- [ ] Pagamento online
- [ ] Rastreamento de entrega
- [ ] Sistema de fidelidade
- [ ] Sugestões personalizadas com ML

### Versão 2.0
- [ ] Multi-tenant (várias pizzarias)
- [ ] Painel administrativo
- [ ] Relatórios e analytics
- [ ] Chat com voz (TTS/STT)
- [ ] Integração com delivery apps

---

## 📞 Suporte e Contribuição

### Reportar Issues
- Abra uma issue no GitHub
- Descreva o problema detalhadamente
- Inclua logs e passos para reproduzir

### Contribuir
1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Commit suas mudanças: `git commit -am 'Adiciona nova feature'`
4. Push para a branch: `git push origin feature/nova-feature`
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 🙏 Agradecimentos

- **Microsoft** - Semantic Kernel
- **Ollama** - Execução local de LLMs
- **PostgreSQL** - Banco de dados robusto
- **Comunidade .NET** - Suporte e recursos

---

**Feito com 🍕 e 💻 por [Seu Nome]**

**Versão**: 1.0.0 | **Data**: Fevereiro/2026

---

## 📚 Referências

### Documentação Oficial
- [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Ollama](https://github.com/ollama/ollama)
- [PostgreSQL](https://www.postgresql.org/docs/)
- [pgvector](https://github.com/pgvector/pgvector)

### Tutoriais Úteis
- [Building RAG with Semantic Kernel](https://devblogs.microsoft.com/semantic-kernel/)
- [Local LLMs with Ollama](https://ollama.ai/blog)
- [Vector Search in PostgreSQL](https://supabase.com/blog/openai-embeddings-postgres-vector)

### Comunidades
- [Discord - Semantic Kernel](https://discord.com/invite/semantic-kernel)
- [Reddit - r/LocalLLaMA](https://www.reddit.com/r/LocalLLaMA/)

---

**Fim do Documento**

*Última atualização: 06/02/2026*
