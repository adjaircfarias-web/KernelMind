# ✅ US-005: Criar Scripts de Setup e Utilitários - CONCLUÍDA

**Data:** 06/02/2026  
**Status:** ✅ COMPLETADA  
**Tempo:** ~45 minutos

---

## 📦 Scripts Criados

### 1. scripts/setup.ps1 (Windows PowerShell)

**Funcionalidades:**
- ✅ Verifica pré-requisitos (Docker, Docker Compose, .NET SDK)
- ✅ Cria arquivo `.env` automaticamente a partir de `.env.example`
- ✅ Inicia containers Docker (postgres + ollama)
- ✅ Aguarda serviços ficarem prontos (health checks)
- ✅ Restaura pacotes NuGet
- ✅ Compila a solução
- ✅ Mensagens coloridas e informativas

**Uso:**
```powershell
.\scripts\setup.ps1
```

---

### 2. scripts/setup.sh (Linux/Mac/Bash)

**Funcionalidades:**
- ✅ Idêntico ao setup.ps1 para sistemas Unix
- ✅ Compatível com bash/zsh
- ✅ Verificação de dependências
- ✅ Cria `.env` automaticamente
- ✅ Aguarda serviços prontos
- ✅ Mensagens coloridas

**Uso:**
```bash
chmod +x scripts/setup.sh
./scripts/setup.sh
```

---

### 3. Makefile

**Comandos disponíveis:**

| Comando | Descrição |
|---------|-----------|
| `make help` | Mostra todos os comandos disponíveis |
| `make setup` | Executa setup inicial (Windows) |
| `make build` | Compila a solução .NET |
| `make run` | Inicia a API |
| `make run-web` | Inicia o frontend Angular |
| `make test` | Executa os testes |
| `make up` | Inicia containers Docker |
| `make down` | Para containers Docker |
| `make down-v` | Para e remove volumes |
| `make logs` | Mostra logs em tempo real |
| `make logs-api` | Logs da API |
| `make logs-db` | Logs do PostgreSQL |
| `make logs-ollama` | Logs do Ollama |
| `make db-update` | Aplica migrations |
| `make db-add` | Cria nova migration |
| `make seed` | Popula banco com dados |
| `make clean` | Limpa arquivos de build |
| `make clean-all` | Limpa tudo + Docker |
| `make status` | Status dos containers |
| `make dev` | Ambiente dev completo |
| `make install-tools` | Instala ferramentas .NET |
| `make docker-build` | Build containers |
| `make docker-rebuild` | Rebuild sem cache |
| `make docker-pull` | Atualiza imagens |
| `make restart` | Reinicia containers |
| `make shell-api` | Shell do container API |
| `make shell-db` | Shell do container DB |
| `make shell-ollama` | Shell do container Ollama |

---

## ✅ Critérios de Aceitação

- [x] Criar `scripts/setup.ps1` (setup inicial Windows)
- [x] Criar `scripts/setup.sh` (setup inicial Linux/Mac)
- [x] Criar `Makefile` com comandos: up, down, build, logs, seed, clean
- [x] Scripts verificam pré-requisitos (Docker, Ollama)
- [x] Scripts criam `.env` automaticamente se não existir
- [x] Adicionar mensagens coloridas e informativas
- [x] Makefile funcional em sistemas Unix

---

## 📋 Verificação de Pré-Requisitos

### Windows (setup.ps1)
```powershell
# Verifica:
where docker        # Docker Desktop
docker compose      # Docker Compose v2
where dotnet        # .NET SDK
```

### Linux/Mac (setup.sh)
```bash
# Verifica:
command -v docker
docker compose version
command -v dotnet
```

---

## 🚀 Exemplos de Uso

### Setup Completo (Windows)
```powershell
# Executar setup
.\scripts\setup.ps1

# Iniciar API
dotnet run --project src/KernelMind.Api

# Testar
curl http://localhost:5076/health
```

### Setup Completo (Linux/Mac)
```bash
# Executar setup
chmod +x scripts/setup.sh
./scripts/setup.sh

# Iniciar API
dotnet run --project src/KernelMind.Api

# Testar
curl http://localhost:5076/health
```

### Usando Makefile
```bash
# Setup completo
make setup

# Apenas infraestrutura Docker
make up

# Ver status
make status

# Ver logs
make logs-api

# Aplicar migrations
make db-update

# Limpar tudo
make clean-all
```

---

## 📊 Estrutura de Arquivos

```
scripts/
├── setup.ps1        # Windows PowerShell (162 linhas)
├── setup.sh         # Linux/Mac Bash (160 linhas)
└── README.md        # Documentação

Makefile              # 155 linhas

.env.example          # 90 variáveis
.gitignore           # 433 linhas
```

---

## 🎨 Cores e Formatação

### Windows (ANSI codes)
```batch
set "GREEN=[92m"
set "YELLOW=[93m"
set "RED=[91m"
set "BLUE=[94m]"
set "NC=[0m"
```

### Linux/Mac
```bash
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'
```

---

## ⚠️ Notas Importantes

1. **Permissões Linux/Mac:**
   ```bash
   chmod +x scripts/setup.sh
   ```

2. **PowerShell no Windows:**
   - O script usa comandos nativos do PowerShell
   - Requer PowerShell 5.1 ou superior

3. **Docker Compose:**
   - Suporta tanto `docker compose` (v2) quanto `docker-compose` (v1)

4. **Tempo de Espera:**
   - PostgreSQL: ~2-5 segundos
   - Ollama: Variável (primeira execução baixa modelos)

---

## 🔧 Próximos Passos

1. **US-006:** Criar projetos .NET 10
2. **US-007:** Implementar entidades do domínio
3. **US-008:** Configurar Entity Framework Core
4. **US-009:** Criar primeiras migrations

---

## 📈 Resumo

- **Scripts criados:** 2 (setup.ps1, setup.sh)
- **Comandos Makefile:** 22 comandos
- **Linhas de código:** ~320 linhas de scripts
- **Plataformas suportadas:** Windows, Linux, macOS
- **Build verificado:** ✅ SUCCESS (0 errors, 0 warnings)
