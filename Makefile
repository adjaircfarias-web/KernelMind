# KernelMind - Makefile
# Comandos úteis para desenvolvimento e operação

.PHONY: help setup build run test clean up down logs seed restart shell-api shell-db shell-ollama

# Default target
.DEFAULT_GOAL := help

# Colors for output
BLUE := \033[36m
GREEN := \033[32m
YELLOW := \033[33m
RED := \033[31m
NC := \033[0m # No Color

help: ## Mostra esta mensagem de ajuda
	@echo "$(BLUE)KernelMind - Comandos Disponíveis$(NC)"
	@echo "======================================"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  $(GREEN)%-15s$(NC) %s\n", $$1, $$2}'

setup: ## Executa o setup inicial do projeto
	@echo "$(BLUE)🚀 Executando setup inicial...$(NC)"
	@powershell -ExecutionPolicy Bypass -File scripts/setup.ps1

build: ## Compila a solução .NET
	@echo "$(BLUE)🔨 Compilando solução...$(NC)"
	dotnet build
	@echo "$(GREEN)✅ Compilação concluída!$(NC)"

build-api: ## Compila apenas o projeto API
	@echo "$(BLUE)🔨 Compilando API...$(NC)"
	dotnet build src/KernelMind.Api/KernelMind.Api.csproj
	@echo "$(GREEN)✅ API compilada!$(NC)"

run: ## Inicia a API
	@echo "$(BLUE)🚀 Iniciando API...$(NC)"
	dotnet run --project src/KernelMind.Api

run-web: ## Inicia o frontend Angular (requer npm install)
	@echo "$(BLUE)🚀 Iniciando Frontend...$(NC)"
	cd src/KernelMind.Web && ng serve

test: ## Executa os testes
	@echo "$(BLUE)🧪 Executando testes...$(NC)"
	dotnet test

up: ## Inicia a infraestrutura Docker (postgres + ollama)
	@echo "$(BLUE)🐳 Iniciando infraestrutura Docker...$(NC)"
	docker-compose up -d postgres ollama
	@echo "$(GREEN)✅ Infraestrutura iniciada!$(NC)"
	@echo "$(YELLOW)⏳ Aguardando serviços ficarem prontos...$(NC)"
	@sleep 5
	@echo "$(GREEN)✅ Pronto!$(NC)"

down: ## Para todos os containers Docker
	@echo "$(BLUE)🛑 Parando containers...$(NC)"
	docker-compose down
	@echo "$(GREEN)✅ Containers parados!$(NC)"

down-v: ## Para containers e remove volumes (⚠️ perde dados!)
	@echo "$(RED)⚠️  Atenção: Isso removerá todos os dados!$(NC)"
	@read -p "Tem certeza? [y/N] " -n 1 -r; \
	echo; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker-compose down -v; \
		echo "$(GREEN)✅ Containers e volumes removidos!$(NC)"; \
	fi

logs: ## Mostra logs de todos os serviços
	docker-compose logs -f

logs-api: ## Mostra logs da API
	docker-compose logs -f backend

logs-db: ## Mostra logs do PostgreSQL
	docker-compose logs -f postgres

logs-ollama: ## Mostra logs do Ollama
	docker-compose logs -f ollama

db-update: ## Aplica as migrations do Entity Framework
	@echo "$(BLUE)🗄️  Aplicando migrations...$(NC)"
	dotnet ef database update --project src/KernelMind.Infrastructure --startup-project src/KernelMind.Api
	@echo "$(GREEN)✅ Migrations aplicadas!$(NC)"

db-add: ## Cria uma nova migration (use: make db-add name=NomeMigration)
	@if [ -z "$(name)" ]; then \
		echo "$(RED)❌ Erro: Especifique o nome da migration$(NC)"; \
		echo "$(YELLOW)Uso: make db-add name=NomeDaMigration$(NC)"; \
		exit 1; \
	fi
	@echo "$(BLUE)🗄️  Criando migration '$(name)'...$(NC)"
	dotnet ef migrations add $(name) --project src/KernelMind.Infrastructure --startup-project src/KernelMind.Api
	@echo "$(GREEN)✅ Migration criada!$(NC)"

clean: ## Limpa arquivos de build e restore
	@echo "$(BLUE)🧹 Limpando arquivos de build...$(NC)"
	dotnet clean
	@find . -type d -name "bin" -o -name "obj" | xargs rm -rf
	@echo "$(GREEN)✅ Limpo!$(NC)"

clean-all: ## Limpa tudo incluindo Docker volumes (⚠️ perde dados!)
	@echo "$(RED)⚠️  Atenção: Isso limpará tudo incluindo dados do Docker!$(NC)"
	@read -p "Tem certeza? [y/N] " -n 1 -r; \
	echo; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		dotnet clean; \
		docker-compose down -v --remove-orphans; \
		docker system prune -f; \
		echo "$(GREEN)✅ Tudo limpo!$(NC)"; \
	fi

status: ## Mostra status dos containers
	@echo "$(BLUE)📊 Status dos containers:$(NC)"
	docker-compose ps

dev: ## Inicia ambiente de desenvolvimento completo (Docker + API)
	@echo "$(BLUE)🚀 Iniciando ambiente de desenvolvimento...$(NC)"
	$(MAKE) up
	@echo "$(GREEN)✅ Infraestrutura pronta!$(NC)"
	@echo "$(BLUE)🚀 Iniciando API...$(NC)"
	@sleep 2
	dotnet run --project src/KernelMind.Api

install-tools: ## Instala ferramentas .NET necessárias
	@echo "$(BLUE)📦 Instalando ferramentas .NET...$(NC)"
	dotnet tool install --global dotnet-ef || echo "$(YELLOW)⚠️  dotnet-ef já instalado$(NC)"
	@echo "$(GREEN)✅ Ferramentas instaladas!$(NC)"

# Docker shortcuts
docker-build: ## Builda todos os containers
	docker-compose build

docker-rebuild: ## Rebuilda todos os containers (sem cache)
	docker-compose build --no-cache

docker-pull: ## Atualiza as imagens Docker
	docker-compose pull

seed: ## Popula o banco com dados iniciais (pizzas)
	@echo "$(BLUE)🌱 Populando banco de dados...$(NC)"
	@echo "$(YELLOW)Nota: Implemente o seed data no projeto$(NC)"
	dotnet run --project src/KernelMind.Api --seed || echo "$(YELLOW)Seed não implementado ainda$(NC)"
	@echo "$(GREEN)✅ Seed concluído!$(NC)"

restart: ## Reinicia todos os containers
	@echo "$(BLUE)🔄 Reiniciando containers...$(NC)"
	docker-compose restart
	@echo "$(GREEN)✅ Containers reiniciados!$(NC)"

shell-api: ## Acessa o shell do container da API
	docker-compose exec backend /bin/sh

shell-db: ## Acessa o shell do container do PostgreSQL
	docker-compose exec postgres /bin/sh

shell-ollama: ## Acessa o shell do container do Ollama
	docker-compose exec ollama /bin/sh
