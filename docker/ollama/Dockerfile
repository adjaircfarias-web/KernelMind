# Ollama - Local LLM Server
# Runs llama3.1:70b and embedding models locally

FROM ollama/ollama:latest

LABEL maintainer="KernelMind Team"
LABEL description="Ollama server for local LLM inference"

# Install curl for healthchecks
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Create necessary directories
RUN mkdir -p /root/.ollama

# Set environment variables
ENV OLLAMA_HOST=0.0.0.0 \
    OLLAMA_PORT=11434 \
    OLLAMA_KEEP_ALIVE=24h \
    OLLAMA_ORIGINS=*

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:11434/api/tags || exit 1

# Expose Ollama port
EXPOSE 11434

# Use default Ollama entrypoint
ENTRYPOINT ["/bin/ollama"]
CMD ["serve"]
