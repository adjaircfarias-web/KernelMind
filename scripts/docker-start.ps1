@echo off
chcp 65001 >nul
echo 🚀 KernelMind - Starting Docker Infrastructure
echo =================================================

REM Check if .env file exists
if not exist .env (
    echo ⚠️  Warning: .env file not found!
    echo Creating from .env.example...
    copy .env.example .env
    echo ✅ Created .env - Please review and customize if needed
    echo.
)

REM Stop any running containers first
echo 🛑 Stopping any existing containers...
docker-compose down --remove-orphans 2>nul

REM Start infrastructure services only (postgres + ollama)
echo.
echo 📦 Starting infrastructure services...
docker-compose up -d postgres ollama

REM Wait for PostgreSQL to be healthy
echo ⏳ Waiting for PostgreSQL to be ready...
:wait_postgres
docker-compose exec -T postgres pg_isready -U postgres >nul 2>&1
if errorlevel 1 (
    timeout /t 2 /nobreak >nul
    goto wait_postgres
)
echo ✅ PostgreSQL is ready!

REM Wait for Ollama
echo ⏳ Waiting for Ollama to be ready...
:wait_ollama
docker-compose exec -T ollama curl -sf http://localhost:11434/api/tags >nul 2>&1
if errorlevel 1 (
    echo    Still downloading models... Please wait...
    timeout /t 10 /nobreak >nul
    goto wait_ollama
)
echo ✅ Ollama is ready!

echo.
echo =================================================
echo ✅ Infrastructure started successfully!
echo.
echo Services:
echo   🗄️  PostgreSQL:  localhost:5432
echo   🤖 Ollama:      localhost:11434
echo.
echo Next steps:
echo   1. Apply database migrations: dotnet ef database update
echo   2. Start backend:  cd src/KernelMind.Api ^&^& dotnet run
echo   3. Start frontend: cd src/KernelMind.Web ^&^& ng serve
echo.
echo Or start all with: docker-compose up -d
echo =================================================
