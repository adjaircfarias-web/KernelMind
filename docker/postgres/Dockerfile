# PostgreSQL 16 with pgvector extension
# Image for vector storage and similarity search

FROM pgvector/pgvector:pg16

LABEL maintainer="KernelMind Team"
LABEL description="PostgreSQL 16 with pgvector extension for RAG"

# Copy custom pg_hba.conf
COPY pg_hba.conf /etc/postgresql/pg_hba.conf

# Copy initialization scripts
COPY init/ /docker-entrypoint-initdb.d/

# Set proper permissions
RUN chmod +x /docker-entrypoint-initdb.d/*.sql 2>/dev/null || true

# Health check
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=3 \
    CMD pg_isready -U postgres || exit 1

# Expose PostgreSQL port
EXPOSE 5432

# Use default postgres entrypoint
CMD ["postgres"]
