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

## 🚀 Início Rápido

### Pré-requisitos
- Docker Desktop
- 16GB RAM mínimo (32GB recomendado)
- 20GB espaço em disco

### Executando

```bash
# Clonar e entrar no diretório
cd KernelMind

# Iniciar todos os serviços (primeira vez baixa ~10GB)
docker-compose up -d

# Acessar a aplicação
# Frontend: http://localhost:4200
# API: http://localhost:5076
# Swagger: http://localhost:5076/swagger
```

### Desenvolvimento Local

```bash
# Backend (.NET)
cd src/KernelMind.Api
dotnet restore
dotnet run

# Frontend (Angular)
cd src/KernelMind.Web
npm install
npm start
```

---

## 📁 Estrutura do Projeto

```
KernelMind/
├── src/
│   ├── KernelMind.Api/          # .NET 10 Web API
│   ├── KernelMind.Core/         # Plugins & Services
│   ├── KernelMind.Domain/       # Entidades & Interfaces
│   ├── KernelMind.Infrastructure/# Repositories & Data
│   └── KernelMind.Web/          # Angular 19 Frontend
├── docker/
│   ├── postgres/               # PostgreSQL + pgvector
│   ├── ollama/                  # Servidor LLM
│   └── nginx/                   # Configuração Nginx
├── tests/
│   ├── KernelMind.UnitTests/    # Testes unitários
│   └── KernelMind.IntegrationTests/ # Testes de integração
├── docs/                        # Documentação
├── docker-compose.yml           # Produção
├── docker-compose.override.yml   # Desenvolvimento
└── README.md
```

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

- [Arquitetura](docs/ARCHITECTURE.md)
- [User Stories](Plan/USER-STORIES.md)
- [API Swagger](http://localhost:5076/swagger)
- [Docker](docs/US-035-COMPLETED.md)

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
