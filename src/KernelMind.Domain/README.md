# KernelMind.Domain

## Purpose
Domain layer containing entities, value objects, interfaces, and pure business rules.
Follows Domain-Driven Design (DDD) principles.

## Responsibilities
- **Entities (records):**
  - Pizza
  - Order
  - OrderItem
  - Customer
  - ChatSession
  - ChatMessage
- **Value Objects (records):**
  - Money
- **Repository Interfaces:**
  - IPizzaRepository
  - IOrderRepository
  - IChatSessionRepository
- **Enums and Constants**

## References
- None (Domain is the innermost layer)

## Expected Structure
```
KernelMind.Domain/
├── Entities/
│   ├── Pizza.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Customer.cs
│   ├── ChatSession.cs
│   └── ChatMessage.cs
├── ValueObjects/
│   └── Money.cs
├── Interfaces/
│   ├── IPizzaRepository.cs
│   ├── IOrderRepository.cs
│   └── IChatSessionRepository.cs
├── Enums/
│   └── OrderStatus.cs
└── README.md
```

## Code Standards
- **Prefer `record` over `class`** for entities and DTOs
- Use `init` setters for immutable properties
- English names: `Pizza`, `Order`, `Customer`

## Useful Commands
```bash
# Create project
dotnet new classlib -n KernelMind.Domain
```
