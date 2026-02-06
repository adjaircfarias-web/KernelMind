# KernelMind.Web

## Purpose
Angular 19 Frontend - User interface for the pizza ordering chatbot.

## Responsibilities
- Interactive chat interface
- API consumption via HTTP Streaming (Fetch API)
- Chat state management
- Menu display
- Order tracking

## Technologies
- Angular 19
- TypeScript 5
- RxJS (reactive programming)
- Angular Material (UI components)
- HTTP Streaming with Fetch API

## Expected Structure
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

## HTTP Streaming
The frontend consumes the API using HTTP Streaming with `IAsyncEnumerable`:
```typescript
// Streaming consumption example
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

## Useful Commands
```bash
# Create project
ng new KernelMind.Web --routing --style=scss

# Install Angular Material
ng add @angular/material

# Serve in development
ng serve

# Build for production
ng build --configuration production
```
