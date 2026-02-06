# KernelMind.Domain

## 📋 Propósito
Camada de domínio contendo entidades, value objects, interfaces e regras de negócio puras.
Segue os princípios de DDD (Domain-Driven Design).

## 📦 Responsabilidades
- **Entidades (records):**
  - Pizza
  - Order
  - OrderItem
  - Customer
  - ChatSession
  - ChatMessage
- **Value Objects (records):**
  - Money
  - Address
- **Interfaces de repositório:**
  - IPizzaRepository
  - IOrderRepository
  - IChatSessionRepository
- **Enums e constantes**

## 🔗 Referências
- Nenhuma (Domain é a camada mais interna)

## 📁 Estrutura Esperada
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
│   ├── Money.cs
│   └── Address.cs
├── Interfaces/
│   ├── IPizzaRepository.cs
│   ├── IOrderRepository.cs
│   └── IChatSessionRepository.cs
├── Enums/
│   └── OrderStatus.cs
└── README.md
```

## 📝 Padrões de Código
- **Preferir `record` ao invés de `class`** para entidades e DTOs
- Usar `init` setters para propriedades imutáveis
- Nomes em inglês: `Pizza`, `Order`, `Customer`

## 🚀 Comandos Úteis
```bash
# Criar projeto
dotnet new classlib -n KernelMind.Domain
```
