# API REST – KernelMind

Referência dos endpoints da API. Para experimentação interativa use o **Swagger**: `http://localhost:5076/swagger`.

---

## Base URL

- Local: `http://localhost:5076`
- Produção: conforme deploy (ex.: `https://api.seudominio.com`)

---

## Chat

| Method | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/chat/message` | Enviar mensagem (resposta completa; suporta function calling) |
| POST | `/api/chat/stream` | Resposta em streaming SSE (eventos em JSON) |
| POST | `/api/chat/stream/raw` | Resposta em streaming SSE (texto puro) |
| GET | `/api/chat/health` | Health check do serviço de chat |

### POST /api/chat/message

**Request (JSON):**
```json
{
  "message": "Quero ver o cardápio",
  "sessionId": "opcional-uuid-ou-token"
}
```

**Response 200:**
```json
{
  "content": "Aqui está nosso cardápio...",
  "sessionId": "abc-123",
  "timestamp": "2026-02-23T12:00:00Z"
}
```

**Validação:** `message` é obrigatório. Em caso de erro: `400` com `{ "error": "Message is required" }`.

---

### Streaming (SSE)

Endpoints `POST /api/chat/stream` e `POST /api/chat/stream/raw` aceitam o mesmo body do `/api/chat/message`. Resposta: `Content-Type: text/event-stream`.

- **`/api/chat/stream`**: cada evento é um JSON, ex.: `data: {"chunk":"...","sessionId":"..."}\n\n`. Final: `data: [DONE]\n\n`. Em erro: um evento `data` com o JSON de erro.
- **`/api/chat/stream/raw`**: cada evento é o texto do chunk: `data: <chunk>\n\n`. Final: `data: [DONE]\n\n`. Cancelamento: `data: [CANCELLED]\n\n`. Erro: `data: ERROR: <mensagem>\n\n`.

---

## Menu

| Method | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/menu` | Lista completa de pizzas disponíveis |
| GET | `/api/menu/{id}` | Pizza por ID (guid) |
| GET | `/api/menu/search?query=` | Busca por nome |
| GET | `/api/menu/semantic-search?query=&threshold=&maxResults=` | Busca semântica (RAG) |
| GET | `/api/menu/hybrid-search?query=&maxResults=` | Busca híbrida |
| GET | `/api/menu/{id}/similar?maxResults=` | Pizzas similares |
| GET | `/api/menu/categories` | Lista de categorias |
| GET | `/api/menu/category/{name}` | Pizzas por categoria |
| GET | `/api/menu/vectorization-status` | Status da vetorização |
| POST | `/api/menu/vectorize` | Vetoriza todo o cardápio |
| POST | `/api/menu/reindex` | Re-vetoriza cardápio |

---

## Orders

| Method | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/orders` | Lista pedidos |
| GET | `/api/orders/{id}` | Detalhes do pedido |
| GET | `/api/orders/customer/{customerId}` | Pedidos por cliente |
| GET | `/api/orders/status/{status}` | Pedidos por status |
| POST | `/api/orders` | Criar pedido |
| PATCH | `/api/orders/{id}/status` | Atualizar status |
| POST | `/api/orders/{id}/cancel` | Cancelar pedido |

**Criar pedido (POST /api/orders)** – body exemplo:
```json
{
  "customerId": "uuid-opcional",
  "customerName": "Nome",
  "deliveryAddress": "Endereço",
  "phone": "11999999999",
  "notes": "opcional",
  "items": [
    { "pizzaId": "uuid-pizza", "quantity": 1, "notes": null }
  ]
}
```

---

## Customers

| Method | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/customers` | Lista clientes |
| GET | `/api/customers/{id}` | Cliente por ID |
| GET | `/api/customers/email/{email}` | Cliente por e-mail |
| GET | `/api/customers/phone/{phone}` | Cliente por telefone |
| POST | `/api/customers` | Criar cliente |
| PUT | `/api/customers/{id}` | Atualizar cliente |

---

## Health

| Method | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/chat/health` | Status do serviço de chat |
| GET | `/health` | Health geral (se configurado) |

---

## CORS

Em **desenvolvimento** a API aceita qualquer origem. Em **produção** são aceitas apenas as origens configuradas em `Cors:AllowedOrigins` (ex.: `http://localhost:4200`, origem do front em produção). Ver [SECURITY.md](SECURITY.md).

---

## Erros comuns

- **400** – Dados inválidos (ex.: `message` vazio no chat, body de pedido inválido).
- **404** – Recurso não encontrado (id de pizza, pedido ou cliente).
- **500** – Erro interno (ex.: falha no LLM ou banco). Mensagem genérica no body; detalhes em logs do servidor.
