# PostgreSQL 16 with pgvector extension
# Image for vector storage and similarity search

FROM postgres:16-alpine

LABEL maintainer="KernelMind Team"
LABEL description="PostgreSQL 16 with pgvector extension for RAG"

# Install build dependencies and pgvector
RUN apk add --no-cache \
    postgresql16-dev \
    build-base \
    git \
    && cd /tmp \
    && git clone --branch v0.8.0 https://github.com/pgvector/pgvector.git \
    && cd pgvector \
    && make \
    && make install \
    && cd / \
    && rm -rf /tmp/pgvector \
    && apk del build-base git postgresql16-dev

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
