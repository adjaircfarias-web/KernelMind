# 🧠 KernelMind

**AI-Powered Pizza Ordering Chatbot**

A complete full-stack application demonstrating:
- 🤖 **Semantic Kernel** with local LLM (Ollama)
- 📚 **RAG (Retrieval Augmented Generation)** with embeddings
- 🔌 **Plugins (Tooling)** for business logic
- 🌐 **Angular 19** frontend
- ⚙️ **.NET 10** backend API
- 🗄️ **PostgreSQL** with pgvector
- 🐳 **Docker Compose** orchestration

---

## 🚀 Quick Start

```bash
# Clone the repository
git clone <repository-url>
cd KernelMind

# Start all services with one command
docker-compose up -d

# Access the application
# Frontend: http://localhost:4200
# API: http://localhost:5076
# Swagger: http://localhost:5076/swagger
```

---

## 📁 Project Structure

```
KernelMind/
├── src/
│   ├── KernelMind.Api/          # .NET 10 Web API
│   ├── KernelMind.Core/         # Business logic & Plugins
│   ├── KernelMind.Domain/       # Domain entities (records)
│   ├── KernelMind.Infrastructure/ # Data access
│   └── KernelMind.Web/          # Angular 19 frontend
├── docker/
│   ├── postgres/                # PostgreSQL + pgvector
│   ├── ollama/                  # LLM container
│   └── nginx/                   # Reverse proxy config
├── scripts/                     # Setup scripts
├── docs/                        # Documentation
└── tests/                       # Test projects
```

---

## 🛠️ Tech Stack

### Backend
- **.NET 10** - Framework
- **Semantic Kernel** - AI orchestration
- **Entity Framework Core** - ORM
- **PostgreSQL** + **pgvector** - Database & vector search

### Frontend
- **Angular 19** - Framework
- **Angular Material** - UI components
- **RxJS** - Reactive programming
- **Fetch API** - HTTP streaming

### Infrastructure
- **Docker** & **Docker Compose**
- **Ollama** - Local LLM (llama3.1:70b)
- **Nginx** - Reverse proxy

---

## 📝 Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for local development)
- [Node.js 20+](https://nodejs.org/) (for local development)
- 48GB RAM recommended (for llama3.1:70b model)

---

## 🐳 Services

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 4200 | Angular web app |
| Backend API | 5076 | .NET Web API |
| PostgreSQL | 5432 | Database with pgvector |
| Ollama | 11434 | Local LLM server |

---

## 📚 Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [User Stories](docs/USER-STORIES.md)
- [API Documentation](http://localhost:5076/swagger) (when running)

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License.

---

**Made with 🍕 and 💻**
