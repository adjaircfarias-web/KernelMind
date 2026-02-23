# Guia de Contribuição – KernelMind

---

## Comece por aqui

1. **Setup:** Siga o [README principal](../README.md) (Como rodar – desenvolvimento local).
2. **Arquitetura:** Leia [ARCHITECTURE.md](ARCHITECTURE.md) para entender camadas e fluxos.
3. **API:** Consulte [API.md](API.md) e o Swagger para contratos e exemplos.

---

## Branches e commits

- **Branch principal:** `main`.
- **Features:** branch a partir de `main`, ex.: `feature/nome-da-feature` ou `fix/descricao-do-fix`.
- **Commits:** mensagens claras, preferencialmente em inglês. Exemplos:
  - `feat: add menu plugin`
  - `fix: resolve order calculation bug`
  - `docs: update API documentation`
  - `refactor: improve chat service performance`

---

## Pull Requests

- Abra PR contra `main` com descrição objetiva do que foi alterado e por quê.
- **Requisitos mínimos:**
  - Testes existentes passando (`dotnet test`).
  - Novas funcionalidades ou mudanças de comportamento cobertas por testes quando fizer sentido.
  - Documentação relevante atualizada (README, docs/, comentários em código).
- Mudanças que afetem arquitetura, contratos de API ou segurança devem ser descritas no PR e, se necessário, refletidas em [ARCHITECTURE.md](ARCHITECTURE.md) e [API.md](API.md).

---

## Padrões de código

- **Backend (.NET):** C# com convenções do projeto; XML docs em inglês para APIs públicas; comentários podem ser em português.
- **API:** Use DTOs para request/response; validação com atributos (ex.: `[Required]`); não expor entidades de domínio diretamente nos controllers.
- **Frontend (Angular):** Componentes standalone; estado compartilhado via serviços (ex.: `OrderStateService`); tipagem TypeScript consistente.
- **IA/Prompts:** Prompts de sistema ficam em `KernelMind.Core/Prompts` (ex.: `ChatPrompts`); regras de tools documentadas na arquitetura ou em docs de IA.

---

## Dúvidas

Em caso de dúvida sobre onde colocar código, convenções ou documentação, abra uma issue ou discuta no PR.
