# Segurança e Operação – KernelMind

Checklist e recomendações para deploy e operação em ambiente controlado.

---

## CORS

- **Desenvolvimento:** a API usa a política `DevCors` (qualquer origem).
- **Produção:** use a política `AppCors`. Configure origens permitidas em `Cors:AllowedOrigins` (appsettings ou variáveis de ambiente), por exemplo:
  - URL do frontend (ex.: `https://app.seudominio.com`)
  - `http://localhost:4200` apenas se necessário em homologação
- Não use `AllowAnyOrigin` em produção.

---

## TLS e proxy

- Em produção, exponha a API e o frontend atrás de um proxy reverso (ex.: Nginx) com **HTTPS**.
- Configure headers de segurança no Nginx (ex.: ver seção Segurança em [ARCHITECTURE.md](ARCHITECTURE.md)):
  - `X-Frame-Options`, `X-Content-Type-Options`, `X-XSS-Protection`, `Referrer-Policy`

---

## Segredos e variáveis de ambiente

- **Nunca** commitar senhas, chaves de API ou tokens no repositório.
- Use `.env` ou variáveis de ambiente do host/container para:
  - `ConnectionStrings__DefaultConnection` (PostgreSQL)
  - Chaves ou segredos de aplicação (ex.: `JWT_SECRET` se usado)
- Em Docker, prefira secrets do Docker ou do orquestrador em vez de variáveis em claro em arquivos versionados.

---

## Banco de dados

- Usuário da aplicação com permissões **mínimas** (SELECT, INSERT, UPDATE, DELETE nas tabelas necessárias; sem DROP/ALTER em produção sem processo controlado).
- Senha forte para o usuário do PostgreSQL; acesso à porta 5432 restrito à rede interna ou ao backend.

---

## Containers e recursos

- Limites de memória/CPU conforme [ARCHITECTURE.md](ARCHITECTURE.md) (Performance – Limites de Recursos) para evitar consumo excessivo.
- Rodar containers com usuário não-root quando possível (já adotado para Nginx no projeto).

---

## Logs e auditoria

- Em produção, não logar dados sensíveis (senhas, tokens, PII completo). Logs estruturados com `sessionId`/correlation para rastreio de erros no chat.

---

Para detalhes de deploy e Docker, consulte [ARCHITECTURE.md](ARCHITECTURE.md) e a documentação em `docker/`.
