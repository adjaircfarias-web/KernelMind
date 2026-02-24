# REST API – KernelMind

API endpoint reference. For interactive exploration use **Swagger**: `http://localhost:5076/swagger`.

---

## Base URL

- Local: `http://localhost:5076`
- Production: as per deployment (e.g. `https://api.yourdomain.com`)

---

## Chat

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/chat/message` | Send message (full response; supports function calling) |
| POST | `/api/chat/stream` | SSE streaming response (JSON events) |
| POST | `/api/chat/stream/raw` | SSE streaming response (plain text) |
| GET | `/api/chat/health` | Chat service health check |

### POST /api/chat/message

**Request (JSON):**
```json
{
  "message": "I want to see the menu",
  "sessionId": "optional-uuid-or-token"
}
```

**Response 200:**
```json
{
  "content": "Here is our menu...",
  "sessionId": "abc-123",
  "timestamp": "2026-02-23T12:00:00Z"
}
```

**Validation:** `message` is required. On error: `400` with `{ "error": "Message is required" }`.

---

### Streaming (SSE)

Endpoints `POST /api/chat/stream` and `POST /api/chat/stream/raw` accept the same body as `/api/chat/message`. Response: `Content-Type: text/event-stream`.

- **`/api/chat/stream`**: each event is JSON, e.g. `data: {"chunk":"...","sessionId":"..."}\n\n`. End: `data: [DONE]\n\n`. On error: one `data` event with error JSON.
- **`/api/chat/stream/raw`**: each event is the chunk text: `data: <chunk>\n\n`. End: `data: [DONE]\n\n`. Cancellation: `data: [CANCELLED]\n\n`. Error: `data: ERROR: <message>\n\n`.

---

## Menu

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/menu` | Full list of available pizzas |
| GET | `/api/menu/{id}` | Pizza by ID (guid) |
| GET | `/api/menu/search?query=` | Search by name |
| GET | `/api/menu/semantic-search?query=&threshold=&maxResults=` | Semantic search (RAG) |
| GET | `/api/menu/hybrid-search?query=&maxResults=` | Hybrid search |
| GET | `/api/menu/{id}/similar?maxResults=` | Similar pizzas |
| GET | `/api/menu/categories` | Category list |
| GET | `/api/menu/category/{name}` | Pizzas by category |
| GET | `/api/menu/vectorization-status` | Vectorization status |
| POST | `/api/menu/vectorize` | Vectorize full menu |
| POST | `/api/menu/reindex` | Re-vectorize menu |

---

## Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | List orders |
| GET | `/api/orders/{id}` | Order details |
| GET | `/api/orders/customer/{customerId}` | Orders by customer |
| GET | `/api/orders/status/{status}` | Orders by status |
| POST | `/api/orders` | Create order |
| PATCH | `/api/orders/{id}/status` | Update status |
| POST | `/api/orders/{id}/cancel` | Cancel order |

**Create order (POST /api/orders)** – example body:
```json
{
  "customerId": "optional-uuid",
  "customerName": "Name",
  "deliveryAddress": "Address",
  "phone": "11999999999",
  "notes": "optional",
  "items": [
    { "pizzaId": "pizza-uuid", "quantity": 1, "notes": null }
  ]
}
```

---

## Customers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customers` | List customers |
| GET | `/api/customers/{id}` | Customer by ID |
| GET | `/api/customers/email/{email}` | Customer by email |
| GET | `/api/customers/phone/{phone}` | Customer by phone |
| POST | `/api/customers` | Create customer |
| PUT | `/api/customers/{id}` | Update customer |

---

## Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/chat/health` | Chat service status |
| GET | `/health` | General health (if configured) |

---

## CORS

In **development** the API allows any origin. In **production** only origins configured in `Cors:AllowedOrigins` are allowed (e.g. `http://localhost:4200`, production frontend origin). See [SECURITY.md](SECURITY.md).

---

## Common Errors

- **400** – Invalid data (e.g. empty `message` in chat, invalid order body).
- **404** – Resource not found (pizza, order or customer id).
- **500** – Internal error (e.g. LLM or database failure). Generic message in body; details in server logs.
