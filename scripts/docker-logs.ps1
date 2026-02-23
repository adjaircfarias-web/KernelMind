@echo off
chcp 65001 >nul
echo 📋 KernelMind - Docker Logs
echo =================================================
echo.

if "%~1"=="" (
    echo Usage: .\docker-logs.ps1 [service] [options]
    echo.
    echo Services:
    echo   postgres    - PostgreSQL database logs
    echo   ollama      - Ollama LLM logs
    echo   backend     - Backend API logs
    echo   frontend    - Frontend logs
    echo   all         - All services (default)
    echo.
    echo Options:
    echo   -f, --follow    Follow log output
    echo   -n, --lines     Number of lines to show
    echo   --tail          Same as --lines
    echo.
    echo Examples:
    echo   .\docker-logs.ps1 postgres -f
    echo   .\docker-logs.ps1 backend -n 100
    echo   .\docker-logs.ps1 all --follow
    echo.
    
    REM Show all logs by default
    docker-compose logs --tail=100 -f
) else (
    REM Pass arguments to docker-compose
    docker-compose logs %*
)
