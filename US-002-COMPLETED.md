# ✅ US-002: Configurar Docker Compose Completo - CONCLUÍDA

**Data:** 06/02/2026  
**Status:** ✅ COMPLETADA  
**Tempo:** ~1 hora 30 minutos

---

## 📦 Arquivos Criados

### 1. docker-compose.yml (Principal)
Orquestração completa com 4 serviços:

```yaml
services:
  - postgres     # PostgreSQL 16 + pgvector (porta 5432)
  - ollama       # LLM Local (porta 11434)
  - backend      # .NET 10 Web API (porta 5076)
  - frontend     # Angular 19 (porta 4200)
```

**Features:**
- ✅ Rede compartilhada `kernelmind-network`
- ✅ Volumes persistentes para dados
- ✅ Healthchecks para todos os serviços
- ✅ Limites de memória configuráveis
- ✅ Dependências entre serviços
- ✅ Configuração via variáveis de ambiente

### 2. docker-compose.override.yml (Desenvolvimento)
Configurações específicas para desenvolvimento:
- Hot reload para .NET e Angular
- Portas mapeadas para localhost
- Volumes para código-fonte (live editing)
- Redução de memória para máquinas de dev

### 3. Dockerfiles

#### docker/postgres/Dockerfile
- Base: postgres:16-alpine
- Instala extensão pgvector v0.8.0
- Scripts de inicialização automática
- Healthcheck configurado

#### docker/ollama/Dockerfile
- Base: ollama/ollama:latest
- Baixa modelos automaticamente
- Configurações de performance
- Healthcheck configurado

#### src/KernelMind.Api/Dockerfile
- Multi-stage build (build, publish, dev, production)
- Estágio de desenvolvimento com hot reload
- Estágio de produção otimizado
- Usuário não-root para segurança

#### src/KernelMind.Web/Dockerfile
- Multi-stage build com Node.js e Nginx
- Build Angular otimizado para produção
- Servidor Nginx com configurações de cache
- Estágio de desenvolvimento com Angular CLI

### 4. Configurações de Suporte

#### docker/postgres/init/01-init.sql
- Schema `kernelmind` completo
- Tabelas: pizzas, customers, orders, order_items, chat_sessions, chat_messages, vector_documents
- Extensão pgvector habilitada
- Índices de vetores para busca semântica
- Funções: `search_pizzas()`, `search_documents()`
- Seed data: 8 pizzas de exemplo + FAQ documents

#### src/KernelMind.Web/nginx.conf
- Configuração otimizada para Angular
- Gzip compression
- Cache de assets estáticos
- Proxy para API backend
- Security headers

### 5. Scripts PowerShell

#### scripts/docker-start.ps1
```powershell
# Uso: .\docker-start.ps1
# Inicia infraestrutura (postgres + ollama)
# Aguarda healthchecks
# Fornece instruções de próximos passos
```

#### scripts/docker-stop.ps1
```powershell
# Uso: .\docker-stop.ps1
# Para todos os containers
# Remove containers órfãos
```

#### scripts/docker-logs.ps1
```powershell
# Uso: .\docker-logs.ps1 [servico] [opcoes]
# Ex: .\docker-logs.ps1 postgres -f
# Ex: .\docker-logs.ps1 backend -n 100
```

### 6. .env.example (Atualizado)
Variáveis de ambiente completas:
- PostgreSQL (DB, User, Password, Port)
- Ollama (URL, Model, Port, Temperature, MaxTokens)
- Backend (Environment, Port, JWT settings)
- Frontend (Port, API URL)
- Docker (Project name, Resource limits)
- Feature flags e development settings

---

## ✅ Critérios de Aceitação

- [x] Criar serviço `frontend` (Angular) na porta 4200
- [x] Criar serviço `backend` (.NET) na porta 5076
- [x] Criar serviço `postgres` (PostgreSQL + pgvector) na porta 5432
- [x] Criar serviço `ollama` (LLM) na porta 11434
- [x] Configurar rede compartilhada `kernelmind-network`
- [x] Configurar volumes persistentes para postgres e ollama
- [x] Adicionar healthchecks para postgres
- [x] Adicionar healthchecks para todos os serviços
- [x] Criar docker-compose.override.yml para desenvolvimento

---

## 🚀 Como Usar

### Iniciar Infraestrutura (Desenvolvimento)
```powershell
# Opção 1: Apenas infra (postgres + ollama)
.\scripts\docker-start.ps1

# Opção 2: Tudo com docker-compose
# Configurar .env primeiro
copy .env.example .env

# Subir todos os serviços
docker-compose up -d

# Ou com override para dev
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

### Verificar Status
```powershell
docker-compose ps
docker-compose logs -f
```

### Parar Tudo
```powershell
.\scripts\docker-stop.ps1
# ou
docker-compose down
```

### Aplicar Migrations (quando backend estiver pronto)
```bash
cd src/KernelMind.Api
dotnet ef database update
```

---

## 🗄️ Estrutura do Banco

O PostgreSQL é inicializado com:
- **Schema:** `kernelmind`
- **Tabelas:**
  - `pizzas` - Cardápio com embeddings vetoriais
  - `customers` - Clientes
  - `orders` - Pedidos
  - `order_items` - Itens dos pedidos
  - `chat_sessions` - Sessões de chat
  - `chat_messages` - Mensagens do chat
  - `vector_documents` - Documentos para RAG
- **Funções:**
  - `search_pizzas()` - Busca semântica de pizzas
  - `search_documents()` - Busca semântica de documentos
- **Dados:** 8 pizzas + 6 documentos FAQ

---

## 📝 Notas Importantes

1. **Portas:** Todas as portas são configuráveis via `.env`
2. **Memória:** Ollama requer significativa memória RAM/VRAM
   - llama3.1:8b → mínimo 4GB
   - llama3.1:70b → mínimo 48GB
3. **Hot Reload:** Override file habilita live editing para dev
4. **Healthchecks:** Todos os serviços têm verificação de saúde
5. **Rede:** Container se comunicam via `kernelmind-network`

---

## 🎯 Próximos Passos

1. **US-003:** Criar projetos .NET (API, Core, Domain, Infrastructure)
2. **US-004:** Criar projeto Angular
3. **US-005:** Configurar Entity Framework e Migrations
4. **Testar:** `docker-compose up -d` deve funcionar completamente

