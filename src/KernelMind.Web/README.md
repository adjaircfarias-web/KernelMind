# KernelMind.Web

## 📋 Propósito
Frontend Angular 19 - Interface do usuário para o chatbot de pedidos de pizza.

## 📦 Responsabilidades
- Interface de chat interativa
- Consumo da API via HTTP Streaming (Fetch API)
- Gerenciamento de estado do chat
- Exibição de cardápio
- Acompanhamento de pedidos

## 🎨 Tecnologias
- Angular 19
- TypeScript 5
- RxJS (reactive programming)
- Angular Material (UI components)
- HTTP Streaming com Fetch API

## 📁 Estrutura Esperada
```
KernelMind.Web/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── chat/
│   │   │   ├── menu/
│   │   │   └── order/
│   │   ├── services/
│   │   │   ├── chat.service.ts
│   │   │   ├── order.service.ts
│   │   │   └── streaming.service.ts
│   │   ├── models/
│   │   │   ├── chat.model.ts
│   │   │   ├── order.model.ts
│   │   │   └── pizza.model.ts
│   │   └── app.component.ts
│   ├── assets/
│   └── environments/
├── angular.json
├── package.json
└── README.md
```

## 🌐 HTTP Streaming
O frontend consome a API usando HTTP Streaming com `IAsyncEnumerable`:
```typescript
// Exemplo de consumo do streaming
async* streamChat(message: string): AsyncGenerator<string> {
  const response = await fetch('/api/chat/stream', {
    method: 'POST',
    body: JSON.stringify({ message })
  });
  
  const reader = response.body?.getReader();
  while (true) {
    const { done, value } = await reader!.read();
    if (done) break;
    yield new TextDecoder().decode(value);
  }
}
```

## 🚀 Comandos Úteis
```bash
# Criar projeto
ng new KernelMind.Web --routing --style=scss

# Instalar Angular Material
ng add @angular/material

# Servir em desenvolvimento
ng serve

# Build para produção
ng build --configuration production
```
