@echo off
chcp 65001 >nul
setlocal EnableDelayedExpansion

:: Colors for output
set "GREEN=[92m"
set "YELLOW=[93m"
set "RED=[91m"
set "BLUE=[94m"
set "NC=[0m"

echo %BLUE%╔══════════════════════════════════════════════════════════════╗%NC%
echo %BLUE%║                                                              ║%NC%
echo %BLUE%║              🧠 KernelMind - Setup Inicial                   ║%NC%
echo %BLUE%║         Chatbot de Pizzas com IA Generativa                  ║%NC%
echo %BLUE%║                                                              ║%NC%
echo %BLUE%╚══════════════════════════════════════════════════════════════╝%NC%
echo.

:: Check if .env exists
if not exist .env (
    echo %YELLOW%⚠️  Arquivo .env não encontrado. Criando a partir de .env.example...%NC%
    copy .env.example .env >nul 2>&1
    if exist .env (
        echo %GREEN%✅ Arquivo .env criado com sucesso!%NC%
        echo %YELLOW%⚠️  Por favor, revise e personalize o arquivo .env se necessário.%NC%
    ) else (
        echo %RED%❌ Erro ao criar arquivo .env%NC%
        exit /b 1
    )
) else (
    echo %GREEN%✅ Arquivo .env encontrado%NC%
)

echo.
echo %BLUE%📋 Verificando pré-requisitos...%NC%

:: Check Docker
where docker >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%a in ('docker --version') do set "DOCKER_VERSION=%%a"
    echo %GREEN%✅ Docker: %DOCKER_VERSION%%NC%
) else (
    echo %RED%❌ Docker não encontrado. Por favor, instale o Docker Desktop.%NC%
    echo    https://www.docker.com/products/docker-desktop
    exit /b 1
)

:: Check Docker Compose
docker compose version >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%a in ('docker compose version') do set "COMPOSE_VERSION=%%a"
    echo %GREEN%✅ Docker Compose: %COMPOSE_VERSION%%NC%
) else (
    echo %RED%❌ Docker Compose não encontrado%NC%
    exit /b 1
)

:: Check .NET SDK
where dotnet >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%a in ('dotnet --version') do set "DOTNET_VERSION=%%a"
    echo %GREEN%✅ .NET SDK: %DOTNET_VERSION%%NC%
) else (
    echo %RED%❌ .NET SDK não encontrado. Por favor, instale o .NET 10 SDK.%NC%
    echo    https://dotnet.microsoft.com/download
    exit /b 1
)

echo.
echo %BLUE%🚀 Iniciando infraestrutura...%NC%

:: Start Docker infrastructure
echo %YELLOW%⏳ Subindo containers Docker...%NC%
docker-compose up -d postgres ollama

if %errorlevel% neq 0 (
    echo %RED%❌ Erro ao iniciar containers Docker%NC%
    exit /b 1
)

:: Wait for PostgreSQL
echo %YELLOW%⏳ Aguardando PostgreSQL...%NC%
:wait_postgres
docker-compose exec -T postgres pg_isready -U postgres >nul 2>&1
if errorlevel 1 (
    timeout /t 2 /nobreak >nul
    goto wait_postgres
)
echo %GREEN%✅ PostgreSQL pronto!%NC%

:: Wait for Ollama
echo %YELLOW%⏳ Aguardando Ollama...%NC%
:wait_ollama
docker-compose exec -T ollama curl -sf http://localhost:11434/api/tags >nul 2>&1
if errorlevel 1 (
    echo %YELLOW%   Baixando modelos... isso pode levar alguns minutos...%NC%
    timeout /t 10 /nobreak >nul
    goto wait_ollama
)
echo %GREEN%✅ Ollama pronto!%NC%

echo.
echo %BLUE%📦 Restaurando pacotes .NET...%NC%
dotnet restore

if %errorlevel% neq 0 (
    echo %RED%❌ Erro ao restaurar pacotes NuGet%NC%
    exit /b 1
)

echo %GREEN%✅ Pacotes restaurados!%NC%

echo.
echo %BLUE%🔨 Compilando solução...%NC%
dotnet build --no-restore

if %errorlevel% neq 0 (
    echo %RED%❌ Erro ao compilar solução%NC%
    exit /b 1
)

echo %GREEN%✅ Compilação bem-sucedida!%NC%

echo.
echo %BLUE%╔══════════════════════════════════════════════════════════════╗%NC%
echo %BLUE%║                                                              ║%NC%
echo %BLUE%║              ✅ Setup Completo!                              ║%NC%
echo %BLUE%║                                                              ║%NC%
echo %BLUE%╚══════════════════════════════════════════════════════════════╝%NC%
echo.
echo %GREEN%🎉 KernelMind está pronto para usar!%NC%
echo.
echo %BLUE%Próximos passos:%NC%
echo   1. Aplicar migrations:    dotnet ef database update --project src/KernelMind.Infrastructure --startup-project src/KernelMind.Api
if %errorlevel% neq 0 (
    echo %YELLOW%      Nota: Entity Framework Core tools não encontrado.%NC%
    echo %YELLOW%      Instale com: dotnet tool install --global dotnet-ef%NC%
)
echo   2. Iniciar API:           dotnet run --project src/KernelMind.Api
if exist "src\KernelMind.Web\package.json" (
    echo   3. Iniciar Frontend:    cd src/KernelMind.Web ^&^& npm install ^&^& ng serve
)
echo.
echo %BLUE%URLs:%NC%
echo   • API:        http://localhost:5076
echo   • Swagger:    http://localhost:5076/swagger
echo   • PostgreSQL: localhost:5432
echo   • Ollama:     localhost:11434
if exist "src\KernelMind.Web\package.json" (
    echo   • Frontend:   http://localhost:4200
)
echo.
echo %YELLOW%Documentação:%NC%
echo   • README.md
if exist "docs\README.md" (
    echo   • docs/README.md
)
echo.

endlocal
