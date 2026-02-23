# Documentation

Technical and business documentation for the KernelMind project.

---

## Comece por aqui

| Objetivo | Documento |
|----------|-----------|
| Entender a arquitetura e o fluxo do sistema | [ARCHITECTURE.md](ARCHITECTURE.md) |
| Consultar endpoints e contratos da API | [API.md](API.md) |
| Rodar o projeto (dev, Docker, produção) | [README principal](../README.md#-como-rodar) |
| Rodar testes | [TESTING.md](TESTING.md) |
| Contribuir (branches, PRs, padrões) | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Deploy e segurança (CORS, TLS, segredos) | [SECURITY.md](SECURITY.md) |

---

## Documentação em docs/

| Arquivo | Descrição |
|---------|-----------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | Arquitetura de camadas, diagramas, Semantic Kernel, RAG, endpoints, schema do banco, Docker, performance, testes |
| [API.md](API.md) | Referência da API REST (Chat, Menu, Orders, Customers, Health); exemplos de request/response e streaming SSE |
| [TESTING.md](TESTING.md) | Onde estão os testes, como rodar (unit + integration), ferramentas e metas de cobertura |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Convenções de commit/branch, requisitos de PR, padrões de código (backend, API, frontend, prompts) |
| [SECURITY.md](SECURITY.md) | CORS, TLS, segredos, banco, limites de recursos e logs em produção |

### Plan/ (na raiz do repo)

- [PLANO-IMPLEMENTACAO.md](../Plan/PLANO-IMPLEMENTACAO.md) – Plano técnico e fases (parte histórica; estado atual reflete Angular + API web)
- [ARQUITETURA-COMPLETA.md](../Plan/ARQUITETURA-COMPLETA.md) – Visão expandida da arquitetura
- [USER-STORIES.md](../Plan/USER-STORIES.md) – User stories (39 US)

---

## Padrões

- **Código:** Comentários podem ser em português; XML Documentation em inglês.
- **Commits:** `feat:`, `fix:`, `docs:`, `refactor:` (ex.: `feat: add menu plugin`).

## Links úteis

- [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/) · [Angular 19](https://angular.io/docs) · [pgvector](https://github.com/pgvector/pgvector) · [Ollama](https://ollama.ai)
