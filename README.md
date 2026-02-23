# 🧠 KernelMind

**Chatbot de Pedidos de Pizza com IA**

Aplicação completa demonstrando:
- 🤖 **Semantic Kernel** com LLM local (Ollama)
- 📚 **RAG (Retrieval Augmented Generation)** com embeddings
- 🔌 **Plugins** para lógica de negócios
- 🌐 **Angular 19** frontend
- ⚙️ **.NET 10** backend API
- 🗄️ **PostgreSQL** com pgvector
- 🐳 **Docker Compose** orquestração

---

## 🚀 Como rodar

### Pré-requisitos
- Docker Desktop (para cenários Docker)
- 16GB RAM mínimo (32GB recomendado para Ollama)
- 20GB espaço em disco

### Cenários de execução

| Cenário | Quando usar | Comandos principais |
|--------|-------------|---------------------|
| **Desenvolvimento local** | Editar código com hot reload (API + Angular em terminais separados) | Backend: `cd src/KernelMind.Api && dotnet run` — Frontend: `cd src/KernelMind.Web && npm install && npm start` |
| **Desenvolvimento com Docker** | Rodar stack completa sem instalar .NET/Node localmente | `docker-compose up -d` (use `docker/` para Postgres + Ollama; API e front podem ser locais) |
| **Produção** | Deploy em servidor | Ver [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) (Docker, Nginx, limites de recursos) e [docs/SECURITY.md](docs/SECURITY.md) |

**Desenvolvimento local (recomendado para codar):**

```bash
# Terminal 1 – Backend
cd src/KernelMind.Api
dotnet restore && dotnet run

# Terminal 2 – Frontend
cd src/KernelMind.Web
npm install && npm start
```

Requer PostgreSQL e Ollama rodando (local ou via Docker). Frontend: http://localhost:4200 | API: http://localhost:5076 | Swagger: http://localhost:5076/swagger

**Tudo com Docker:**

```bash
cd KernelMind
docker-compose up -d
# Frontend: http://localhost:4200  |  API: http://localhost:5076
```

---

## 📁 Estrutura do Projeto

| Pasta | Descrição |
|-------|-----------|
| [src/KernelMind.Api](src/KernelMind.Api) | .NET 10 Web API (Controllers, Filters) |
| [src/KernelMind.Core](src/KernelMind.Core) | Plugins, Services, Prompts (Semantic Kernel, Chat, RAG) |
| [src/KernelMind.Domain](src/KernelMind.Domain) | Entidades e interfaces de domínio |
| [src/KernelMind.Infrastructure](src/KernelMind.Infrastructure) | EF Core, repositórios, migrações |
| [src/KernelMind.Web](src/KernelMind.Web) | Frontend Angular 19 |
| [tests](tests) | Testes unitários e de integração |
| [docs](docs) | Documentação (arquitetura, API, testes, segurança) |
| [docker](docker) | Configurações PostgreSQL, Ollama, Nginx |
| [Plan](Plan) | Planos e user stories |

Detalhes da arquitetura: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

---

## 🛠️ Stack Tecnológica

### Backend
| Tecnologia | Propósito |
|------------|-----------|
| .NET 10 | Framework |
| Semantic Kernel | Orquestração IA |
| Entity Framework Core | ORM |
| PostgreSQL + pgvector | Banco vetorial |

### Frontend
| Tecnologia | Propósito |
|------------|-----------|
| Angular 19 | Framework |
| RxJS | Programação reativa |
| SSE | Streaming |

### Infraestrutura
| Tecnologia | Propósito |
|------------|-----------|
| Docker Compose | Orquestração |
| Ollama | LLM local (llama3.1) |
| Nginx | Proxy reverso |

---

## 🤖 Plugins do Semantic Kernel

### MenuPlugin
- `list_menu()` → Cardápio completo
- `search_pizza()` → Busca por nome
- `get_pizza_details()` → Detalhes

### OrderPlugin
- `create_order()` → Novo pedido
- `add_item_to_order()` → Adicionar itens
- `confirm_order()` → Confirmar
- `cancel_order()` → Cancelar

### CalculationPlugin
- `calculate_total()` → Total com entrega
- `calculate_delivery_fee()` → Taxa por distância
- `apply_discount()` → Cupons

### ContextPlugin
- `set_context()` → Salvar contexto
- `get_context()` → Recuperar
- `get_history()` → Histórico

---

## 📚 RAG (Retrieval Augmented Generation)

### Pipeline
```
1. Texto → Embedding (768 dimensões)
2. Busca semântica (pgvector)
3. Contexto recuperado → LLM
4. Resposta gerada
```

### Endpoints RAG
```
GET /api/menu/semantic-search?q=pizza+queijo
GET /api/menu/{id}/similar
POST /api/menu/vectorize
POST /api/menu/reindex
```

---

## 🐳 Serviços Docker

| Serviço | Porta | Descrição |
|---------|--------|-----------|
| Frontend | 4200/80 | Angular dev / Nginx |
| Backend | 5076 | API REST |
| PostgreSQL | 5432 | Banco de dados |
| Ollama | 11434 | Servidor LLM |

---

## 📋 Documentação

- **Arquitetura (fonte da verdade):** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- **Referência da API:** [docs/API.md](docs/API.md) — Swagger: http://localhost:5076/swagger
- **Testes:** [docs/TESTING.md](docs/TESTING.md)
- **Contribuição:** [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)
- **Segurança e operação:** [docs/SECURITY.md](docs/SECURITY.md)
- [User Stories](Plan/USER-STORIES.md) | [Docker](docs/US-035-COMPLETED.md)

---

## 🧪 Testes

```bash
# Testes unitários
dotnet test tests/KernelMind.UnitTests

# Testes de integração
dotnet test tests/KernelMind.IntegrationTests

# Todos os testes
dotnet test
```

**Cobertura**: 46 testes (31 unit + 15 integration)

---

## 🔧 Configuração

### Variáveis de Ambiente (.env)
```env
POSTGRES_PASSWORD=postgres123
OLLAMA_MODEL=llama3.1:8b
BACKEND_PORT=5076
FRONTEND_PORT=4200
JWT_SECRET=sua-chave-secreta
```

---

## 📊 Status do Projeto

```
Fase 0 (Setup):        ✅ 5/5 (100%)
Fase 1 (Core):         ✅ 6/6 (100%)
Fase 2 (Semantic Kernel): ✅ 6/6 (100%)
Fase 3 (RAG):          ✅ 5/5 (100%)
Fase 4 (API):          ✅ 4/4 (100%)
Fase 5 (Frontend):     ✅ 8/8 (100%)
Fase 6 (Integração):   ✅ 5/5 (100%)

TOTAL: 39/39 (100%)
```

---

## 📄 Licença

MIT License

---

**Feito com 🍕 e 💻**
