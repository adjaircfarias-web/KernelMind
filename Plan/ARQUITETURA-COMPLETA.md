# 🧠 KernelMind - Arquitetura Completa (Atualização)

**Adendo ao Plano de Implementação - Frontend Angular + Docker Compose**

---

## 🏗️ Arquitetura Atualizada

### Visão Geral da Arquitetura Full Stack

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         DOCKER COMPOSE ORQUESTRAÇÃO                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────┐    ┌──────────────────────────┐              │
│  │   🌐 FRONTEND ANGULAR    │    │   ⚙️  BACKEND API        │              │
│  │                          │    │                          │              │
│  │  ┌──────────────────┐   │    │  ┌──────────────────┐     │              │
│  │  │   Angular 19     │   │◄──►│  │  .NET 10 Web API │     │              │
│  │  │   TypeScript     │   │HTTP│  │  Semantic Kernel │     │              │
│  │  │   RxJS           │   │Stream│  Plugins         │     │              │
│  │  │   HTTP Client    │   │    │  │  RAG Service     │     │              │
│  │  └──────────────────┘   │    │  └──────────────────┘     │              │
│  │                          │    │           │               │              │
│  │  Porta: 4200            │    │  Porta: 5076              │              │
│  └──────────────────────────┘    └───────────┼───────────────┘              │
│                                              │                              │
│                                              ▼                              │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │                    🗄️  INFRAESTRUTURA DE DADOS                       │ │
│  │                                                                       │ │
│  │  ┌─────────────────────┐    ┌─────────────────────────────────────┐  │ │
│  │  │  🐘 PostgreSQL      │    │  🤖 Ollama (LLM Local)             │  │ │
│  │  │  + pgvector         │    │                                     │  │ │
│  │  │                     │    │  • llama3.1:70b (Chat)             │  │ │
│  │  │  • Pizzas          │    │  • nomic-embed-text (Embeddings)   │  │ │
│  │  │  • Pedidos         │    │                                     │  │ │
│  │  │  • Conversas       │    │  Porta: 11434                      │  │ │
│  │  │  • Embeddings      │    │                                     │  │ │
│  │  │                     │    └─────────────────────────────────────┘  │ │
│  │  │  Porta: 5432       │                                             │ │
│  │  └─────────────────────┘                                             │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
│                                                                             │
│  🔗 REDE DOCKER: kernelmind-network (todos os containers se comunicam)      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 📁 Estrutura de Pastas Atualizada (Full Stack)

```
KernelMind/
├── 📁 src/
│   ├── 📁 KernelMind.Api/                 # BACKEND (.NET 10)
│   │   ├── Controllers/
│   │   │   ├── ChatController.cs       # HTTP com streaming IAsyncEnumerable
│   │   │   ├── PedidoController.cs
│   │   │   └── MenuController.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Dockerfile
│   │
│   ├── 📁 KernelMind.Core/                # NÚCLEO (reutilizado)
│   │   ├── Plugins/
│   │   ├── Services/
│   │   └── Configuration/
│   │
│   ├── 📁 KernelMind.Domain/              # DOMÍNIO
│   │   └── Entidades...
│   │
│   ├── 📁 KernelMind.Infrastructure/      # INFRAESTRUTURA
│   │   ├── Data/
│   │   └── Repositories/
│   │
│   └── 📁 KernelMind.Web/                 # FRONTEND (NOVO - Angular 19)
│       ├── 📁 src/
│       │   ├── 📁 app/
│       │   │   ├── 📁 components/
│       │   │   │   ├── chat/
│       │   │   │   │   ├── chat.component.ts
│       │   │   │   │   ├── chat.component.html
│       │   │   │   │   ├── chat.component.scss
│       │   │   │   │   └── chat.component.spec.ts
│       │   │   │   ├── menu/
│       │   │   │   ├── pedido/
│       │   │   │   └── shared/
│       │   │   │
│       │   │   ├── 📁 services/
│       │   │   │   ├── chat.service.ts         # HTTP + Streaming
│       │   │   │   ├── streaming.service.ts    # Consumo de IAsyncEnumerable
│       │   │   │   └── menu.service.ts
│       │   │   │
│       │   │   ├── 📁 models/
│       │   │   │   ├── pizza.model.ts
│       │   │   │   ├── pedido.model.ts
│       │   │   │   └── mensagem.model.ts
│       │   │   │
│       │   │   ├── app.component.ts
│       │   │   ├── app.config.ts
│       │   │   └── app.routes.ts
│       │   │
│       │   ├── 📁 assets/
│       │   │   └── images/
│       │   │
│       │   ├── 📁 environments/
│       │   │   ├── environment.ts
│       │   │   └── environment.prod.ts
│       │   │
│       │   ├── index.html
│       │   ├── main.ts
│       │   └── styles.scss
│       │
│       ├── angular.json
│       ├── package.json
│       ├── tsconfig.json
│       ├── nginx.conf                      # Configuração nginx para produção
│       └── Dockerfile
│
├── 📁 docker/                              # CONFIGURAÇÕES DOCKER
│   ├── 📁 postgres/
│   │   ├── Dockerfile
│   │   └── init/
│   │       └── 01-init.sql
│   │
│   ├── 📁 ollama/
│   │   └── Dockerfile
│   │
│   └── 📁 nginx/
│       └── default.conf
│
├── 📁 docs/
├── 📁 scripts/
│   ├── setup-dev.sh
│   └── seed-data.ps1
│
├── 📁 tests/
│
├── 📄 docker-compose.yml                    # ORQUESTRAÇÃO COMPLETA
├── 📄 docker-compose.override.yml           # Override para desenvolvimento
├── 📄 .env.example                          # Variáveis de ambiente
├── 📄 Makefile                              # Comandos utilitários
└── 📄 README.md
```

---

## 🌐 Frontend Angular - HTTP Streaming

### Stack do Frontend

| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| Angular | 19.x | Framework frontend |
| TypeScript | 5.x | Linguagem |
| RxJS | 7.x | Programação reativa |
| Angular Material | 19.x | Componentes UI |
| Fetch API | Built-in | Streaming HTTP (IAsyncEnumerable) |
| HTTP Client | Built-in | Requisições HTTP padrão |

### Componentes Principais

#### 1. Chat Component (Principal)
```typescript
// chat.component.ts - Estrutura prevista
@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {
  mensagens: MensagemChat[] = [];
  mensagemAtual: string = '';
  respostaStream: string = '';  // Resposta sendo construída em tempo real
  estaDigitando: boolean = false;
  
  constructor(
    private chatService: ChatService,
    private streamingService: StreamingService
  ) {}
  
  async enviarMensagem() {
    // Envia mensagem e recebe resposta via streaming HTTP
    this.respostaStream = '';
    this.estaDigitando = true;
    
    await this.streamingService.enviarMensagemStream(
      this.mensagemAtual,
      (chunk) => {
        this.respostaStream += chunk;  // Vai construindo a resposta
      }
    );
    
    this.estaDigitando = false;
  }
}
```

#### 2. Interface do Chat
```html
<!-- chat.component.html - Preview -->
<div class="chat-container">
  <div class="chat-header">
    🧠 KernelMind - Pizzaria Inteligente
  </div>
  
  <div class="chat-messages" #scrollContainer>
    <div *ngFor="let msg of mensagens" 
         [class.user]="msg.role === 'user'"
         [class.bot]="msg.role === 'assistant'">
      <div class="message-content" [innerHTML]="msg.conteudo"></div>
      <div class="message-time">{{ msg.timestamp | date:'short' }}</div>
    </div>
    
    <div *ngIf="estaDigitando" class="typing-indicator">
      <span></span><span></span><span></span>
    </div>
  </div>
  
  <div class="chat-input">
    <input [(ngModel)]="mensagemAtual" 
           (keyup.enter)="enviarMensagem()"
           placeholder="Digite sua mensagem...">
    <button (click)="enviarMensagem()" [disabled]="!mensagemAtual.trim()">
      Enviar
    </button>
  </div>
</div>
```

#### 3. Serviços

**ChatService** - Comunicação HTTP com API:
```typescript
@Injectable({ providedIn: 'root' })
export class ChatService {
  private apiUrl = '/api';
  
  constructor(private http: HttpClient) {}
  
  enviarMensagem(mensagem: string): Observable<RespostaChat> {
    return this.http.post<RespostaChat>(`${this.apiUrl}/chat`, { mensagem });
  }
  
  obterCardapio(): Observable<Pizza[]> {
    return this.http.get<Pizza[]>(`${this.apiUrl}/menu`);
  }
  
  criarPedido(pedido: Pedido): Observable<PedidoConfirmado> {
    return this.http.post<PedidoConfirmado>(`${this.apiUrl}/pedidos`, pedido);
  }
}
```

**StreamingService** - Consumo de IAsyncEnumerable:
```typescript
@Injectable({ providedIn: 'root' })
export class StreamingService {
  private apiUrl = '/api';
  
  async enviarMensagemStream(
    mensagem: string, 
    onChunk: (chunk: string) => void
  ): Promise<void> {
    const response = await fetch(`${this.apiUrl}/chat/stream`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ mensagem })
    });
    
    const reader = response.body!.getReader();
    const decoder = new TextDecoder();
    
    while (true) {
      const { done, value } = await reader.read();
      if (done) break;
      
      const chunk = decoder.decode(value);
      onChunk(chunk);  // Callback para atualizar UI em tempo real
    }
  }
}
```

---

## 🔧 Backend API - HTTP Streaming com IAsyncEnumerable

### Controller com Streaming

```csharp
// Controllers/ChatController.cs
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace KernelMind.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Endpoint de chat com streaming de resposta
    /// Retorna IAsyncEnumerable para streaming palavra por palavra
    /// </summary>
    [HttpPost("stream")]
    public async IAsyncEnumerable<string> PostStream(
        [FromBody] MensagemRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Chama o Semantic Kernel com streaming
        await foreach (var token in _chatService.StreamChatAsync(
            request.Mensagem, 
            request.SessaoId ?? Guid.NewGuid().ToString(),
            cancellationToken))
        {
            yield return token;  // Envia cada token assim que é gerado
            await Task.Yield();  // Força flush imediato para o cliente
        }
    }

    /// <summary>
    /// Endpoint simples (sem streaming) para compatibilidade
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MensagemRequest request)
    {
        var resposta = await _chatService.ProcessarMensagemAsync(
            request.Mensagem, 
            request.SessaoId ?? Guid.NewGuid().ToString()
        );

        return Ok(new { resposta });
    }
}

// Modelos de request
public record MensagemRequest(string Mensagem, string? SessaoId = null);
```

### ChatService com Streaming

```csharp
// Services/ChatService.cs
public class ChatService
{
    private readonly Kernel _kernel;
    private readonly ILogger<ChatService> _logger;

    public ChatService(Kernel kernel, ILogger<ChatService> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    /// <summary>
    /// Processa mensagem e ret streaming de tokens
    /// </summary>
    public async IAsyncEnumerable<string> StreamChatAsync(
        string mensagem, 
        string sessaoId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando mensagem em streaming: {Mensagem}", mensagem);

        // Configura função de streaming
        var streamFunction = _kernel.CreateFunctionFromPrompt(
            @"Você é um atendente de pizzaria. Responda de forma amigável.
               
               Histórico: {{$history}}
               Cliente: {{$input}}
               Atendente:""",
            executionSettings: new OpenAIPromptExecutionSettings 
            { 
                MaxTokens = 500,
                Temperature = 0.7
            }
        );

        // Invoca com streaming
        var arguments = new KernelArguments();
        arguments["input"] = mensagem;
        arguments["history"] = await GetHistoryAsync(sessaoId);

        await foreach (var content in _kernel.InvokeStreamingAsync<string>(
            streamFunction, 
            arguments,
            cancellationToken))
        {
            yield return content ?? "";
        }

        // Salva no histórico após completar
        await SaveToHistoryAsync(sessaoId, mensagem, respostaCompleta);
    }

    private async Task<string> GetHistoryAsync(string sessaoId)
    {
        // Recupera histórico do banco
        return "";
    }

    private async Task SaveToHistoryAsync(string sessaoId, string userMsg, string assistantMsg)
    {
        // Salva no banco
    }
}
```

### Outros Controllers

```csharp
// Controllers/ChatController.cs
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MensagemRequest request)
    {
        var resposta = await _chatService.ProcessarMensagemAsync(
            request.Mensagem, 
            request.SessaoId ?? Guid.NewGuid().ToString()
        );

        return Ok(new { resposta });
    }
}

// Controllers/MenuController.cs
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IPizzaRepository _pizzaRepo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pizzas = await _pizzaRepo.GetAllAsync();
        return Ok(pizzas);
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string termo)
    {
        var pizzas = await _vectorSearch.SearchAsync(termo);
        return Ok(pizzas);
    }
}

// Controllers/PedidoController.cs
[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoRepository _pedidoRepo;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PedidoRequest request)
    {
        var pedido = await _pedidoRepo.CreateAsync(request);
        return Ok(pedido);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(id);
        return pedido == null ? NotFound() : Ok(pedido);
    }
}
```

---

## 🐳 Docker Compose - Orquestração Completa

### Arquivo Principal: docker-compose.yml

```yaml
version: '3.8'

services:
  # ============================================
  # FRONTEND - Angular
  # ============================================
  frontend:
    build:
      context: ./src/KernelMind.Web
      dockerfile: Dockerfile
      target: production
    container_name: kernelmind-frontend
    ports:
      - "4200:80"
    environment:
      - API_URL=http://localhost:5076
    depends_on:
      - backend
    networks:
      - kernelmind-network
    restart: unless-stopped

  # ============================================
  # BACKEND - .NET 10 API
  # ============================================
  backend:
    build:
      context: ./src/KernelMind.Api
      dockerfile: Dockerfile
    container_name: kernelmind-backend
    ports:
      - "5076:5076"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=kernelmind;Username=postgres;Password=${POSTGRES_PASSWORD:-postgres123}
      - Ollama__Url=http://ollama:11434
      - Ollama__ChatModel=llama3.1:70b
      - Ollama__EmbeddingModel=nomic-embed-text
    depends_on:
      postgres:
        condition: service_healthy
      ollama:
        condition: service_started
    networks:
      - kernelmind-network
    restart: unless-stopped

  # ============================================
  # BANCO DE DADOS - PostgreSQL + pgvector
  # ============================================
  postgres:
    build:
      context: ./docker/postgres
      dockerfile: Dockerfile
    container_name: kernelmind-postgres
    environment:
      POSTGRES_DB: kernelmind
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres123}
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./docker/postgres/init:/docker-entrypoint-initdb.d
    networks:
      - kernelmind-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

  # ============================================
  # MODELO DE LINGUAGEM - Ollama
  # ============================================
  ollama:
    build:
      context: ./docker/ollama
      dockerfile: Dockerfile
    container_name: kernelmind-ollama
    ports:
      - "11434:11434"
    volumes:
      - ollama_data:/root/.ollama
    networks:
      - kernelmind-network
    # Pull automático dos modelos na inicialização
    entrypoint: >
      sh -c "
        ollama serve &
        sleep 10
        ollama pull llama3.1:70b
        ollama pull nomic-embed-text
        tail -f /dev/null
      "
    restart: unless-stopped
    deploy:
      resources:
        limits:
          memory: 48G
        reservations:
          memory: 24G

# ============================================
# VOLUMES
# ============================================
volumes:
  postgres_data:
    driver: local
  ollama_data:
    driver: local

# ============================================
# REDE
# ============================================
networks:
  kernelmind-network:
    driver: bridge
```

### Docker Compose Override (Desenvolvimento)

```yaml
# docker-compose.override.yml
version: '3.8'

services:
  frontend:
    build:
      target: development
    volumes:
      - ./src/KernelMind.Web:/app
      - /app/node_modules
    ports:
      - "4200:4200"
    command: ng serve --host 0.0.0.0 --poll 2000
    environment:
      - CHOKIDAR_USEPOLLING=true

  backend:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    volumes:
      - ./src/KernelMind.Api:/app
      - /app/bin
      - /app/obj
```

### Dockerfiles

#### Frontend (Angular)

```dockerfile
# src/KernelMind.Web/Dockerfile

# -----------------------------------------
# ESTÁGIO 1: BUILD
# -----------------------------------------
FROM node:20-alpine AS build

WORKDIR /app

# Instala dependências
COPY package*.json ./
RUN npm ci --legacy-peer-deps

# Copia código fonte
COPY . .

# Build de produção
RUN npm run build -- --configuration production

# -----------------------------------------
# ESTÁGIO 2: PRODUÇÃO (Nginx)
# -----------------------------------------
FROM nginx:alpine AS production

# Remove configuração padrão
RUN rm /etc/nginx/conf.d/default.conf

# Copia configuração customizada
COPY nginx.conf /etc/nginx/conf.d/

# Copia build do Angular
COPY --from=build /app/dist/kernel-mind/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

# -----------------------------------------
# ESTÁGIO 3: DESENVOLVIMENTO
# -----------------------------------------
FROM node:20-alpine AS development

WORKDIR /app

COPY package*.json ./
RUN npm ci --legacy-peer-deps

COPY . .

EXPOSE 4200

CMD ["npm", "start"]
```

#### Backend (.NET 10)

```dockerfile
# src/KernelMind.Api/Dockerfile

# -----------------------------------------
# ESTÁGIO 1: BUILD
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copia e restaura dependências
COPY ["KernelMind.Api/KernelMind.Api.csproj", "KernelMind.Api/"]
COPY ["KernelMind.Core/KernelMind.Core.csproj", "KernelMind.Core/"]
COPY ["KernelMind.Domain/KernelMind.Domain.csproj", "KernelMind.Domain/"]
COPY ["KernelMind.Infrastructure/KernelMind.Infrastructure.csproj", "KernelMind.Infrastructure/"]
RUN dotnet restore "KernelMind.Api/KernelMind.Api.csproj"

# Copia todo o código e builda
COPY . .
WORKDIR "/src/KernelMind.Api"
RUN dotnet build "KernelMind.Api.csproj" -c Release -o /app/build

# -----------------------------------------
# ESTÁGIO 2: PUBLISH
# -----------------------------------------
FROM build AS publish
RUN dotnet publish "KernelMind.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------
# ESTÁGIO 3: RUNTIME
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copia publicação
COPY --from=publish /app/publish .

EXPOSE 5076

ENTRYPOINT ["dotnet", "KernelMind.Api.dll"]
```

#### PostgreSQL com pgvector

```dockerfile
# docker/postgres/Dockerfile
FROM postgres:16-alpine

# Instala extensão pgvector
RUN apk add --no-cache --virtual .build-deps \
    git \
    build-base \
    clang15 \
    llvm15 \
    && git clone --branch v0.5.1 https://github.com/pgvector/pgvector.git \
    && cd pgvector \
    && make \
    && make install \
    && cd .. \
    && rm -rf pgvector \
    && apk del .build-deps

# Copia scripts de inicialização
COPY init/ /docker-entrypoint-initdb.d/
```

#### Ollama

```dockerfile
# docker/ollama/Dockerfile
FROM ollama/ollama:latest

# Expõe porta
EXPOSE 11434

# O entrypoint é gerenciado pelo docker-compose
```

---

## 🚀 Comandos de Execução

### Makefile Útil

```makefile
# Makefile - Comandos KernelMind

.PHONY: up down build logs shell-backend shell-frontend seed clean

# Subir todos os serviços
up:
	docker-compose up -d

# Subir em modo desenvolvimento
up-dev:
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Parar todos os serviços
down:
	docker-compose down

# Parar e remover volumes
down-v:
	docker-compose down -v

# Rebuildar todas as imagens
build:
	docker-compose build --no-cache

# Ver logs
tlogs:
	docker-compose logs -f

# Logs do backend
logs-backend:
	docker-compose logs -f backend

# Logs do frontend
logs-frontend:
	docker-compose logs -f frontend

# Acessar shell do backend
shell-backend:
	docker-compose exec backend /bin/sh

# Acessar shell do frontend
shell-frontend:
	docker-compose exec frontend /bin/sh

# Popular banco com dados iniciais
seed:
	docker-compose exec backend dotnet run --project /app/seed/SeedData.csproj

# Limpar tudo (⚠️ DESTRUTIVO)
clean:
	docker-compose down -v
	docker system prune -f
	docker volume prune -f

# Status dos containers
status:
	docker-compose ps

# URL de acesso
urls:
	@echo "🌐 Frontend: http://localhost:4200"
	@echo "⚙️  Backend API: http://localhost:5076"
	@echo "📊 Swagger: http://localhost:5076/swagger"
	@echo "🗄️  PostgreSQL: localhost:5432"
	@echo "🤖 Ollama: http://localhost:11434"
```

### Scripts de Setup

```powershell
# scripts/setup.ps1 - Setup inicial completo
Write-Host "🧠 KernelMind - Setup Inicial" -ForegroundColor Cyan

# Verificar Docker
Write-Host "🔍 Verificando Docker..." -ForegroundColor Yellow
docker --version
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Docker não encontrado. Por favor instale o Docker Desktop."
    exit 1
}

# Criar .env se não existir
if (-not (Test-Path .env)) {
    Write-Host "📝 Criando arquivo .env..." -ForegroundColor Yellow
    Copy-Item .env.example .env
}

# Subir infraestrutura
Write-Host "🐳 Iniciando containers..." -ForegroundColor Yellow
docker-compose up -d postgres

# Aguardar PostgreSQL
Write-Host "⏳ Aguardando PostgreSQL..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Executar migrations
Write-Host "🗄️  Executando migrations..." -ForegroundColor Yellow
docker-compose exec backend dotnet ef database update --project KernelMind.Infrastructure --startup-project KernelMind.Api

# Popular dados
Write-Host "🍕 Populando cardápio..." -ForegroundColor Yellow
docker-compose exec backend dotnet run --seed

# Subir restante
Write-Host "🚀 Iniciando Ollama e Frontend..." -ForegroundColor Yellow
docker-compose up -d ollama frontend

Write-Host "✅ Setup completo!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Acesse: http://localhost:4200" -ForegroundColor Cyan
Write-Host "⚙️  API: http://localhost:5076/swagger" -ForegroundColor Cyan
```

---

## 📋 Fases de Implementação Atualizadas

### FASE 0: Setup Inicial (Dia 0)
- [ ] Criar estrutura de pastas completa (frontend + backend)
- [ ] Configurar Docker Compose com todos os serviços
- [ ] Criar Dockerfiles otimizados
- [ ] Configurar variáveis de ambiente (.env)
- [ ] Criar Makefile e scripts de setup
- [ ] Testar `docker-compose up` funcionando

### FASE 1: Backend Core (Dias 1-3)
- [ ] Configurar projetos .NET 10
- [ ] Implementar entidades e DbContext
- [ ] Criar repositórios
- [ ] Configurar Entity Framework + PostgreSQL
- [ ] Criar seed data do cardápio
- [ ] Testar migrations

### FASE 2: Semantic Kernel (Dias 4-7)
- [ ] Configurar Semantic Kernel
- [ ] Integrar com Ollama
- [ ] Implementar 4 Plugins (Menu, Pedido, Calculo, Contexto)
- [ ] Criar ChatService
- [ ] Testar plugins isoladamente

### FASE 3: RAG (Dias 8-10)
- [ ] Configurar pgvector
- [ ] Implementar EmbeddingService
- [ ] Implementar VectorSearchService
- [ ] Criar pipeline de vetorização
- [ ] Integrar RAG no fluxo de chat
- [ ] Testar buscas semânticas

### FASE 4: API REST com Streaming (Dias 11-13)
- [ ] Criar Controllers (Chat, Menu, Pedido)
- [ ] Implementar ChatController com IAsyncEnumerable
- [ ] Configurar CORS para frontend
- [ ] Criar DTOs e validações
- [ ] Documentar API (Swagger)
- [ ] Testar endpoints com streaming

### FASE 5: Frontend Angular (Dias 14-18)
- [ ] Criar projeto Angular 19
- [ ] Configurar Angular Material
- [ ] Implementar ChatComponent
- [ ] Criar serviços (HTTP + Streaming)
- [ ] Implementar StreamingService com Fetch API
- [ ] Implementar interface de chat com atualização em tempo real
- [ ] Adicionar estilos e animações
- [ ] Testar integração com backend

### FASE 6: Integração e Deploy (Dias 19-20)
- [ ] Configurar Docker Compose completo
- [ ] Testar orquestração de todos os serviços
- [ ] Otimizar builds
- [ ] Escrever documentação
- [ ] Criar testes E2E
- [ ] Deploy local com um comando

---

## 🔗 Comunicação entre Serviços

### Fluxo de Dados

```
Usuário (Browser)
    │
    ├─► Frontend Angular (localhost:4200)
    │     • Interface de chat
    │     • HTTP Client para API
    │     • Fetch API para streaming
    │
    ▼
Backend API (localhost:5076)
    • Controllers REST (IAsyncEnumerable)
    • HTTP Streaming
    • Semantic Kernel
    • Plugins
    │
    ├─► PostgreSQL (porta 5432)
    │     • Dados estruturados
    │     • Embeddings vetoriais
    │
    └─► Ollama (porta 11434)
          • llama3.1:70b (chat)
          • nomic-embed-text (embeddings)
```

### Configuração de CORS

```csharp
// Backend - Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",      // Dev
                "http://localhost",           // Produção nginx
                "http://frontend"             // Docker network
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ...

app.UseCors("AllowFrontend");
```

---

## 📡 Por que HTTP Streaming em vez de WebSocket/SignalR?

### Comparação das Abordagens

| Aspecto | HTTP + Streaming | WebSocket/SignalR |
|---------|------------------|-------------------|
| **Complexidade** | ⭐ Baixa | ⭐⭐⭐ Alta |
| **Overhead** | ⭐ Mínimo | ⭐⭐ Moderado |
| **Setup** | ⭐ Nativo HTTP | ⭐⭐ Configuração extra |
| **Escalabilidade** | ⭐⭐⭐ Excelente | ⭐⭐ Boa |
| **Reconexão** | ⭐ Automática | ⭐⭐⭐ Manual |
| **Bidirecional** | ❌ Não (não precisamos) | ✅ Sim |

### Por que HTTP Streaming é suficiente?

**Para este projeto específico:**

1. **Unidirecional**: O LLM só envia resposta (server → client), não precisamos de comunicação constante do cliente

2. **Simplificado**: 
   - Sem bibliotecas extras (SignalR client)
   - Sem gerenciamento de conexões persistentes
   - Sem problemas de proxy/firewall

3. **Nativo do Browser**:
   ```typescript
   // Funciona em qualquer browser moderno
   const response = await fetch('/api/chat/stream', {method: 'POST'});
   const reader = response.body.getReader();
   ```

4. **Funciona com HTTP/2**: 
   - Multiplexação nativa
   - Melhor performance que WebSocket em muitos casos

5. **Debug facilitado**:
   - Testável via curl
   - Inspecionável no DevTools
   - Logs mais claros

### Quando usar WebSocket/SignalR?

❌ **NÃO precisamos neste projeto:**
- Chat bidirecional em tempo real (tipo WhatsApp)
- Notificações push do servidor
- Múltiplos usuários na mesma sala
- Games online

✅ **HTTP Streaming é suficiente:**
- Resposta do LLM palavra por palavra
- Upload/download de arquivos
- Progresso de operações
- Server-Sent Events (SSE)

### Implementação Técnica

**Backend:**
```csharp
[HttpPost("stream")]
public IAsyncEnumerable<string> PostStream([FromBody] Request request)
{
    // Cada yield retorna imediatamente para o cliente
    yield return "Olá";
    yield return " ";
    yield return "mundo";
}
```

**Frontend:**
```typescript
const response = await fetch('/api/chat/stream', {method: 'POST'});
const reader = response.body.getReader();

while (true) {
  const { done, value } = await reader.read();
  if (done) break;
  
  // Atualiza UI em tempo real
  this.resposta += new TextDecoder().decode(value);
}
```

---

## ✅ Checklist Final

Antes de começar a programar:

- [ ] Plano revisado e aprovado
- [ ] Estrutura de pastas definida
- [ ] Docker Compose configurado
- [ ] Variáveis de ambiente mapeadas
- [ ] Comunicação entre serviços planejada
- [ ] Fases de implementação claras
- [ ] Stack tecnológico confirmado

---

**Documento Complementar ao PLANO-IMPLEMENTACAO.md Principal**

*Atualização: Frontend Angular + HTTP Streaming + Docker Compose Orquestrado*

*Data: 06/02/2026*

---

## 📝 Resumo das Alterações

### ✅ **ATUALIZAÇÃO: Removido SignalR/WebSocket**

**Alterações realizadas nesta versão:**

1. **❌ Removido:** SignalR Hub (ChatHub.cs)
2. **❌ Removido:** SignalR Client do frontend
3. **❌ Removido:** Dependência @microsoft/signalr
4. **✅ Adicionado:** HTTP Streaming com `IAsyncEnumerable<string>`
5. **✅ Adicionado:** Fetch API no Angular para consumir stream
6. **✅ Adicionado:** `StreamingService` no frontend
7. **✅ Adicionado:** Explicação detalhada sobre por que HTTP é suficiente

**Vantagens da nova abordagem:**
- ✅ Código mais simples
- ✅ Sem bibliotecas extras
- ✅ Debug facilitado
- ✅ Performance equivalente para este caso de uso
- ✅ Funciona com HTTP/2 nativo
