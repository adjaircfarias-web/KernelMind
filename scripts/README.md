# Scripts

## 📋 Propósito
Scripts de automação para desenvolvimento, deployment e operações.

## 📁 Scripts Disponíveis

### `docker-start.ps1`
Inicia todos os containers Docker necessários para desenvolvimento.
```powershell
.\docker-start.ps1
```

### `docker-stop.ps1`
Para todos os containers Docker.
```powershell
.\docker-stop.ps1
```

### `docker-logs.ps1`
Exibe logs dos containers em tempo real.
```powershell
.\docker-logs.ps1
```

## 🚀 Comandos Úteis

### Setup Inicial
```powershell
# 1. Iniciar infraestrutura
.\docker-start.ps1

# 2. Aguardar PostgreSQL estar pronto
# (verificar healthcheck)

# 3. Aplicar migrations
cd ..\src\KernelMind.Api
dotnet ef database update

# 4. Iniciar API
dotnet run
```

### Desenvolvimento
```powershell
# Reset completo
.\docker-stop.ps1
.\docker-start.ps1

# Ver status
docker-compose ps
```
