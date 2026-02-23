@echo off
chcp 65001 >nul
echo 🛑 KernelMind - Stopping Docker Infrastructure
echo =================================================

REM Stop all services
docker-compose down --remove-orphans

if %errorlevel% equ 0 (
    echo.
    echo ✅ All services stopped successfully!
) else (
    echo.
    echo ❌ Error stopping services
    exit /b 1
)

echo.
echo Options:
echo   To stop and remove volumes: docker-compose down -v
echo   To stop all containers:     docker stop $(docker ps -q)
echo =================================================
