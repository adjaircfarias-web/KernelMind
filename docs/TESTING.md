# Testing Strategy – KernelMind

---

## Where Tests Live

```
tests/
├── KernelMind.UnitTests/       # Unit tests (xUnit + Moq)
└── KernelMind.IntegrationTests/ # Integration tests (EF Core InMemory, APIs)
```

- **Unit:** services, plugins, value objects, isolated business rules.
- **Integration:** repositories, DbContext, endpoints (when applicable) against a controlled environment.

---

## How to Run

```bash
# From repository root

# Unit only
dotnet test tests/KernelMind.UnitTests

# Integration only
dotnet test tests/KernelMind.IntegrationTests

# All tests
dotnet test
```

For more detail (verbosity, filters):
```bash
dotnet test --logger "console;verbosity=detailed"
dotnet test --filter "FullyQualifiedName~MenuPlugin"
```

---

## Tools

- **xUnit** – test runner and assertions
- **Moq** – mocks in unit tests
- **EF Core InMemory** – in-memory database in integration tests
- **WebApplicationFactory** (if used) – in-memory server endpoint tests

---

## Current Coverage

- **Unit:** ~31 tests (plugins, services, value objects).
- **Integration:** ~15 tests (repositories, database flows).
- **Total:** ~46 tests.

Critical areas prioritized: Semantic Kernel plugins, chat and RAG services, repositories, and API contracts (DTOs/validation).

---

## Goals and Best Practices

- Keep tests stable and fast; integration tests should avoid external services (Ollama, real PostgreSQL) when possible.
- New API features or business rules should include tests (unit and/or integration as appropriate).
- On failure, `dotnet test` output and code under test should be enough to diagnose; avoid obscure logic in tests.

To contribute tests, see [CONTRIBUTING.md](CONTRIBUTING.md).
