# Documentation

Technical and business documentation for the KernelMind project.

---

## Start Here

| Goal | Document |
|------|----------|
| Understand architecture and system flow | [ARCHITECTURE.md](ARCHITECTURE.md) |
| Look up API endpoints and contracts | [API.md](API.md) |
| Run the project (dev, Docker, production) | [Main README](../README.md#-how-to-run) |
| Run tests | [TESTING.md](TESTING.md) |
| Contribute (branches, PRs, standards) | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Deploy and security (CORS, TLS, secrets) | [SECURITY.md](SECURITY.md) |

---

## Documentation in docs/

| File | Description |
|------|-------------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | Layered architecture, diagrams, Semantic Kernel, RAG, endpoints, database schema, Docker, performance, testing |
| [API.md](API.md) | REST API reference (Chat, Menu, Orders, Customers, Health); request/response examples and SSE streaming |
| [TESTING.md](TESTING.md) | Where tests live, how to run (unit + integration), tools and coverage goals |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Commit/branch conventions, PR requirements, code standards (backend, API, frontend, prompts) |
| [SECURITY.md](SECURITY.md) | CORS, TLS, secrets, database, resource limits and logging in production |

### Plan/ (at repo root)

- [PLANO-IMPLEMENTACAO.md](../Plan/PLANO-IMPLEMENTACAO.md) – Technical plan and phases (historical; current state reflects Angular + web API)
- [ARQUITETURA-COMPLETA.md](../Plan/ARQUITETURA-COMPLETA.md) – Expanded architecture view
- [USER-STORIES.md](../Plan/USER-STORIES.md) – User stories (39 US)

---

## Standards

- **Code:** Comments may be in Portuguese; XML Documentation in English.
- **Commits:** `feat:`, `fix:`, `docs:`, `refactor:` (e.g. `feat: add menu plugin`).

## Useful Links

- [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/) · [Angular 19](https://angular.io/docs) · [pgvector](https://github.com/pgvector/pgvector) · [Ollama](https://ollama.ai)
