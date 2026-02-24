# Contributing Guide – KernelMind

---

## Start Here

1. **Setup:** Follow the [main README](../README.md) (How to run – local development).
2. **Architecture:** Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand layers and flows.
3. **API:** Check [API.md](API.md) and Swagger for contracts and examples.

---

## Branches and Commits

- **Main branch:** `main`.
- **Features:** branch from `main`, e.g. `feature/feature-name` or `fix/fix-description`.
- **Commits:** clear messages, preferably in English. Examples:
  - `feat: add menu plugin`
  - `fix: resolve order calculation bug`
  - `docs: update API documentation`
  - `refactor: improve chat service performance`

---

## Pull Requests

- Open a PR against `main` with a clear description of what changed and why.
- **Minimum requirements:**
  - Existing tests passing (`dotnet test`).
  - New features or behavior changes covered by tests when appropriate.
  - Relevant documentation updated (README, docs/, code comments).
- Changes that affect architecture, API contracts or security should be described in the PR and, if needed, reflected in [ARCHITECTURE.md](ARCHITECTURE.md) and [API.md](API.md).

---

## Code Standards

- **Backend (.NET):** C# following project conventions; XML docs in English for public APIs; comments may be in Portuguese.
- **API:** Use DTOs for request/response; validation with attributes (e.g. `[Required]`); do not expose domain entities directly in controllers.
- **Frontend (Angular):** Standalone components; shared state via services (e.g. `OrderStateService`); consistent TypeScript typing.
- **AI/Prompts:** System prompts live in `KernelMind.Core/Prompts` (e.g. `ChatPrompts`); tool rules documented in architecture or AI docs.

---

## Questions

If unsure about where to put code, conventions or documentation, open an issue or discuss in the PR.
