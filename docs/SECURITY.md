# Security and Operations – KernelMind

Checklist and recommendations for deployment and operation in a controlled environment.

---

## CORS

- **Development:** the API uses the `DevCors` policy (any origin).
- **Production:** use the `AppCors` policy. Configure allowed origins in `Cors:AllowedOrigins` (appsettings or environment variables), for example:
  - Frontend URL (e.g. `https://app.yourdomain.com`)
  - `http://localhost:4200` only if needed in staging
- Do not use `AllowAnyOrigin` in production.

---

## TLS and Proxy

- In production, expose the API and frontend behind a reverse proxy (e.g. Nginx) with **HTTPS**.
- Configure security headers in Nginx (see Security section in [ARCHITECTURE.md](ARCHITECTURE.md)):
  - `X-Frame-Options`, `X-Content-Type-Options`, `X-XSS-Protection`, `Referrer-Policy`

---

## Secrets and Environment Variables

- **Never** commit passwords, API keys or tokens to the repository.
- Use `.env` or host/container environment variables for:
  - `ConnectionStrings__DefaultConnection` (PostgreSQL)
  - Application keys or secrets (e.g. `JWT_SECRET` if used)
- With Docker, prefer Docker or orchestrator secrets over plain variables in versioned files.

---

## Database

- Application user with **minimum** permissions (SELECT, INSERT, UPDATE, DELETE on required tables; no DROP/ALTER in production without a controlled process).
- Strong password for the PostgreSQL user; port 5432 access restricted to internal network or backend only.

---

## Containers and Resources

- Memory/CPU limits as in [ARCHITECTURE.md](ARCHITECTURE.md) (Performance – Resource Limits) to avoid excessive usage.
- Run containers as non-root when possible (already in place for Nginx in this project).

---

## Logging and Audit

- In production, do not log sensitive data (passwords, tokens, full PII). Use structured logs with `sessionId`/correlation for chat error tracing.

---

For deployment and Docker details, see [ARCHITECTURE.md](ARCHITECTURE.md) and the documentation in `docker/`.
