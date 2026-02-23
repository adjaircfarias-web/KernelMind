# Estratégia de Testes – KernelMind

---

## Onde estão os testes

```
tests/
├── KernelMind.UnitTests/       # Testes unitários (xUnit + Moq)
└── KernelMind.IntegrationTests/ # Testes de integração (EF Core InMemory, APIs)
```

- **Unitários:** serviços, plugins, value objects, regras de negócio isoladas.
- **Integração:** repositórios, DbContext, endpoints (quando aplicável) contra ambiente controlado.

---

## Como rodar

```bash
# Na raiz do repositório

# Apenas unitários
dotnet test tests/KernelMind.UnitTests

# Apenas integração
dotnet test tests/KernelMind.IntegrationTests

# Todos os testes
dotnet test
```

Para mais detalhes (verbosidade, filtros):
```bash
dotnet test --logger "console;verbosity=detailed"
dotnet test --filter "FullyQualifiedName~MenuPlugin"
```

---

## Ferramentas

- **xUnit** – runner e asserções
- **Moq** – mocks em testes unitários
- **EF Core InMemory** – banco em memória nos testes de integração
- **WebApplicationFactory** (se usado) – testes de endpoints com servidor em memória

---

## Cobertura atual

- **Unit:** ~31 testes (plugins, serviços, value objects).
- **Integration:** ~15 testes (repositórios, fluxos com banco).
- **Total:** ~46 testes.

Áreas críticas priorizadas: plugins do Semantic Kernel, serviços de chat e RAG, repositórios e contratos de API (DTOs/validação).

---

## Metas e boas práticas

- Manter testes estáveis e rápidos; integração sem dependência de serviços externos (Ollama, PostgreSQL real) quando possível.
- Novos recursos de API ou regras de negócio devem vir acompanhados de testes (unit e/ou integração conforme o caso).
- Em caso de falha, logs do `dotnet test` e do código sob teste devem ser suficientes para diagnosticar; evitar lógica obscura nos testes.

Para contribuir com testes, veja [CONTRIBUTING.md](CONTRIBUTING.md).
