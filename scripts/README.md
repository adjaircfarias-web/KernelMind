# Scripts

## Purpose
Automation scripts for development, deployment, and operations.

## Available Scripts

### `docker-start.ps1`
Starts all Docker containers needed for development.
```powershell
.\docker-start.ps1
```

### `docker-stop.ps1`
Stops all Docker containers.
```powershell
.\docker-stop.ps1
```

### `docker-logs.ps1`
Displays container logs in real-time.
```powershell
.\docker-logs.ps1
```

## Useful Commands

### Initial Setup
```powershell
# 1. Start infrastructure
.\docker-start.ps1

# 2. Wait for PostgreSQL to be ready
# (check healthcheck)

# 3. Apply migrations
cd ..\src\KernelMind.Api
dotnet ef database update

# 4. Start API
dotnet run
```

### Development
```powershell
# Complete reset
.\docker-stop.ps1
.\docker-start.ps1

# Check status
docker-compose ps
```

### Using Makefile
```bash
make setup        # Complete setup
make up          # Start Docker
make down        # Stop Docker
make logs        # View logs
make db-update   # Apply migrations
```
