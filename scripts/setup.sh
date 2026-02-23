#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║                                                              ║${NC}"
echo -e "${BLUE}║              🧠 KernelMind - Setup Inicial                   ║${NC}"
echo -e "${BLUE}║         Chatbot de Pizzas com IA Generativa                  ║${NC}"
echo -e "${BLUE}║                                                              ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Check if .env exists
if [ ! -f ".env" ]; then
    echo -e "${YELLOW}⚠️  Arquivo .env não encontrado. Criando a partir de .env.example...${NC}"
    cp .env.example .env 2>/dev/null
    if [ -f ".env" ]; then
        echo -e "${GREEN}✅ Arquivo .env criado com sucesso!${NC}"
        echo -e "${YELLOW}⚠️  Por favor, revise e personalize o arquivo .env se necessário.${NC}"
    else
        echo -e "${RED}❌ Erro ao criar arquivo .env${NC}"
        exit 1
    fi
else
    echo -e "${GREEN}✅ Arquivo .env encontrado${NC}"
fi

echo ""
echo -e "${BLUE}📋 Verificando pré-requisitos...${NC}"

# Check Docker
if command -v docker &> /dev/null; then
    DOCKER_VERSION=$(docker --version)
    echo -e "${GREEN}✅ Docker: ${DOCKER_VERSION}${NC}"
else
    echo -e "${RED}❌ Docker não encontrado. Por favor, instale o Docker.${NC}"
    echo "   https://www.docker.com/products/docker-desktop"
    exit 1
fi

# Check Docker Compose
if docker compose version &> /dev/null; then
    COMPOSE_VERSION=$(docker compose version)
    echo -e "${GREEN}✅ Docker Compose: ${COMPOSE_VERSION}${NC}"
elif docker-compose --version &> /dev/null; then
    COMPOSE_VERSION=$(docker-compose --version)
    echo -e "${GREEN}✅ Docker Compose (v1): ${COMPOSE_VERSION}${NC}"
else
    echo -e "${RED}❌ Docker Compose não encontrado${NC}"
    exit 1
fi

# Check .NET SDK
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✅ .NET SDK: ${DOTNET_VERSION}${NC}"
else
    echo -e "${RED}❌ .NET SDK não encontrado. Por favor, instale o .NET 10 SDK.${NC}"
    echo "   https://dotnet.microsoft.com/download"
    exit 1
fi

echo ""
echo -e "${BLUE}🚀 Iniciando infraestrutura...${NC}"

# Start Docker infrastructure
echo -e "${YELLOW}⏳ Subindo containers Docker...${NC}"
docker-compose up -d postgres ollama

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Erro ao iniciar containers Docker${NC}"
    exit 1
fi

# Wait for PostgreSQL
echo -e "${YELLOW}⏳ Aguardando PostgreSQL...${NC}"
while ! docker-compose exec -T postgres pg_isready -U postgres &> /dev/null; do
    sleep 2
done
echo -e "${GREEN}✅ PostgreSQL pronto!${NC}"

# Wait for Ollama
echo -e "${YELLOW}⏳ Aguandando Ollama...${NC}"
while ! docker-compose exec -T ollama curl -sf http://localhost:11434/api/tags &> /dev/null; do
    echo -e "${YELLOW}   Baixando modelos... isso pode levar alguns minutos...${NC}"
    sleep 10
done
echo -e "${GREEN}✅ Ollama pronto!${NC}"

echo ""
echo -e "${BLUE}📦 Restaurando pacotes .NET...${NC}"
dotnet restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Erro ao restaurar pacotes NuGet${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Pacotes restaurados!${NC}"

echo ""
echo -e "${BLUE}🔨 Compilando solução...${NC}"
dotnet build --no-restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Erro ao compilar solução${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Compilação bem-sucedida!${NC}"

echo ""
echo -e "${BLUE}╔══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║                                                              ║${NC}"
echo -e "${BLUE}║              ✅ Setup Completo!                              ║${NC}"
echo -e "${BLUE}║                                                              ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${GREEN}🎉 KernelMind está pronto para usar!${NC}"
echo ""
echo -e "${BLUE}Próximos passos:${NC}"
echo "   1. Aplicar migrations:    dotnet ef database update --project src/KernelMind.Infrastructure --startup-project src/KernelMind.Api"
echo "   2. Iniciar API:          dotnet run --project src/KernelMind.Api"
if [ -f "src/KernelMind.Web/package.json" ]; then
    echo "   3. Iniciar Frontend:     cd src/KernelMind.Web && npm install && ng serve"
fi
echo ""
echo -e "${BLUE}URLs:${NC}"
echo "   • API:        http://localhost:5076"
echo "   • Swagger:    http://localhost:5076/swagger"
echo "   • PostgreSQL: localhost:5432"
echo "   • Ollama:     localhost:11434"
if [ -f "src/KernelMind.Web/package.json" ]; then
    echo "   • Frontend:   http://localhost:4200"
fi
echo ""
echo -e "${YELLOW}Documentação:${NC}"
echo "   • README.md"
if [ -f "docs/README.md" ]; then
    echo "   • docs/README.md"
fi
echo ""
