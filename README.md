# 🧠 KernelMind

**AI-Powered Pizza Order Chatbot**

Full application demonstrating:
- 🤖 **Semantic Kernel** with local LLM (Ollama)
- 📚 **RAG (Retrieval Augmented Generation)** with embeddings
- 🔌 **Plugins** for business logic
- 🌐 **Angular 19** frontend
- ⚙️ **.NET 10** backend API
- 🗄️ **PostgreSQL** with pgvector
- 🐳 **Docker Compose** orchestration

---

## 🚀 How to Run

### Prerequisites
- Docker Desktop (for Docker scenarios)
- 16GB RAM minimum (32GB recommended for Ollama)
- 20GB disk space

### Execution Scenarios

| Scenario | When to use | Main commands |
|----------|-------------|----------------|
| **Local development** | Edit code with hot reload (API + Angular in separate terminals) | Backend: `cd src/KernelMind.Api && dotnet run` — Frontend: `cd src/KernelMind.Web && npm install && npm start` |
| **Docker development** | Run full stack without installing .NET/Node locally | `docker-compose up -d` (use `docker/` for Postgres + Ollama; API and frontend can be local) |
| **Production** | Deploy to server | See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) (Docker, Nginx, resource limits) and [docs/SECURITY.md](docs/SECURITY.md) |

**Local development (recommended for coding):**

```bash
# Terminal 1 – Backend
cd src/KernelMind.Api
dotnet restore && dotnet run

# Terminal 2 – Frontend
cd src/KernelMind.Web
npm install && npm start
```

Requires PostgreSQL and Ollama running (locally or via Docker). Frontend: http://localhost:4201 | API: http://localhost:5076 | Swagger: http://localhost:5076/swagger

**Full Docker:**

```bash
cd KernelMind
docker-compose up -d
# Frontend: http://localhost:4201  |  API: http://localhost:5076
```

---

## 📁 Project Structure

| Folder | Description |
|--------|-------------|
| [src/KernelMind.Api](src/KernelMind.Api) | .NET 10 Web API (Controllers, Filters) |
| [src/KernelMind.Core](src/KernelMind.Core) | Plugins, Services, Prompts (Semantic Kernel, Chat, RAG) |
| [src/KernelMind.Domain](src/KernelMind.Domain) | Domain entities and interfaces |
| [src/KernelMind.Infrastructure](src/KernelMind.Infrastructure) | EF Core, repositories, migrations |
| [src/KernelMind.Web](src/KernelMind.Web) | Angular 19 frontend |
| [tests](tests) | Unit and integration tests |
| [docs](docs) | Documentation (architecture, API, testing, security) |
| [docker](docker) | PostgreSQL, Ollama, Nginx configuration |
| [Plan](Plan) | Plans and user stories |

Architecture details: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

---

## 🛠️ Technology Stack

### Backend
| Technology | Purpose |
|------------|---------|
| .NET 10 | Framework |
| Semantic Kernel | AI orchestration |
| Microsoft.Extensions.AI | AI abstraction layer |
| Entity Framework Core | ORM |
| PostgreSQL + pgvector | Vector database |

### Frontend
| Technology | Purpose |
|------------|---------|
| Angular 19 | Framework |
| RxJS | Reactive programming |
| SSE | Streaming |

### Infrastructure
| Technology | Purpose |
|------------|---------|
| Docker Compose | Orchestration |
| Ollama | Local LLM (llama3.2:3b) |
| Nginx | Reverse proxy |

---

## 🤖 Semantic Kernel Plugins

### MenuPlugin
- `list_menu()` → Full menu
- `search_pizza()` → Search by name
- `get_pizza_details()` → Details

### OrderPlugin
- `create_order()` → New order
- `add_item_to_order()` → Add items
- `confirm_order()` → Confirm
- `cancel_order()` → Cancel

### CalculationPlugin
- `calculate_total()` → Total with delivery
- `calculate_delivery_fee()` → Distance-based fee
- `apply_discount()` → Coupons

### ContextPlugin
- `set_context()` → Save context
- `get_context()` → Retrieve
- `get_history()` → History

---

## 📚 RAG (Retrieval Augmented Generation)

### Pipeline
```
1. Text → Embedding (768 dimensions)
2. Semantic search (pgvector)
3. Retrieved context → LLM
4. Generated response
```

### RAG Endpoints
```
GET /api/menu/semantic-search?q=pizza+cheese
GET /api/menu/{id}/similar
POST /api/menu/vectorize
POST /api/menu/reindex
```

---

## 🐳 Docker Services

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 4201/80 | Angular dev / Nginx |
| Backend | 5076 | REST API |
| PostgreSQL | 5432 | Database |
| Ollama | 11434 | LLM server |

---

## 📋 Documentation

- **Architecture (source of truth):** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- **API reference:** [docs/API.md](docs/API.md) — Swagger: http://localhost:5076/swagger
- **Testing:** [docs/TESTING.md](docs/TESTING.md)
- **Contributing:** [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)
- **Security and operations:** [docs/SECURITY.md](docs/SECURITY.md)
- [User Stories](Plan/USER-STORIES.md) | [Docker](docs/US-035-COMPLETED.md)

---

## 🧪 Tests

```bash
# Unit tests
dotnet test tests/KernelMind.UnitTests

# Integration tests
dotnet test tests/KernelMind.IntegrationTests

# All tests
dotnet test
```

**Coverage:** 46 tests (31 unit + 15 integration)

---

## 🔧 Configuration

### Environment Variables (.env)
```env
POSTGRES_PASSWORD=postgres123
OLLAMA_MODEL=llama3.2:3b
BACKEND_PORT=5076
FRONTEND_PORT=4201
JWT_SECRET=your-secret-key
```

---

## 📊 Project Status

```
Phase 0 (Setup):        ✅ 5/5 (100%)
Phase 1 (Core):         ✅ 6/6 (100%)
Phase 2 (Semantic Kernel): ✅ 6/6 (100%)
Phase 3 (RAG):          ✅ 5/5 (100%)
Phase 4 (API):          ✅ 4/4 (100%)
Phase 5 (Frontend):     ✅ 8/8 (100%)
Phase 6 (Integration):  ✅ 5/5 (100%)

TOTAL: 39/39 (100%)
```

---

## 📄 License

MIT License

---

**Made with 🍕 and 💻**
