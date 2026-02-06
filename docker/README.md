# Docker Configuration

## 📋 Propósito
Configurações Docker para todos os serviços do projeto KernelMind.

## 📁 Estrutura
```
docker/
├── postgres/          # PostgreSQL + pgvector
├── ollama/           # LLM Local (llama3.1:70b)
└── nginx/            # Reverse Proxy (produção)
```

## 🐳 Serviços

### PostgreSQL + pgvector
- **Imagem:** postgres:16-alpine com pgvector
- **Porta:** 5432
- **Extensão:** pgvector para embeddings vetoriais
- **Volume:** postgres_data

### Ollama
- **Imagem:** ollama/ollama:latest
- **Porta:** 11434
- **Modelo:** llama3.1:70b (baixado na primeira execução)
- **Volume:** ollama_data

### Nginx (Produção)
- **Imagem:** nginx:alpine
- **Porta:** 80/443
- **Função:** Reverse proxy e load balancer

## 🚀 Comandos Úteis
```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down

# Rebuild
docker-compose up -d --build
```
