# Docker Configuration

## Purpose
Docker configurations for all KernelMind project services.

## Structure
```
docker/
├── postgres/          # PostgreSQL + pgvector
├── ollama/           # Local LLM (llama3.1:70b)
└── nginx/            # Reverse Proxy (production)
```

## Services

### PostgreSQL + pgvector
- **Image:** postgres:16-alpine with pgvector
- **Port:** 5432
- **Extension:** pgvector for vector embeddings
- **Volume:** postgres_data

### Ollama
- **Image:** ollama/ollama:latest
- **Port:** 11434
- **Model:** llama3.1:70b (downloaded on first run)
- **Volume:** ollama_data

### Nginx (Production)
- **Image:** nginx:alpine
- **Port:** 80/443
- **Function:** Reverse proxy and load balancer

## Useful Commands
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Rebuild
docker-compose up -d --build
```
