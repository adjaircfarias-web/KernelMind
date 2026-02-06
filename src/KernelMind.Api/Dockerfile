# KernelMind API - .NET 10 Web API
# Multi-stage build for optimized production image

# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for better caching
COPY KernelMind.Api.csproj ./
COPY ../KernelMind.Core/KernelMind.Core.csproj ../KernelMind.Core/
COPY ../KernelMind.Domain/KernelMind.Domain.csproj ../KernelMind.Domain/
COPY ../KernelMind.Infrastructure/KernelMind.Infrastructure.csproj ../KernelMind.Infrastructure/

# Restore dependencies
RUN dotnet restore KernelMind.Api.csproj

# Copy source code
COPY . .
COPY ../KernelMind.Core/ ../KernelMind.Core/
COPY ../KernelMind.Domain/ ../KernelMind.Domain/
COPY ../KernelMind.Infrastructure/ ../KernelMind.Infrastructure/

# Build application
RUN dotnet build KernelMind.Api.csproj -c Release -o /app/build

# ============================================
# Stage 2: Publish
# ============================================
FROM build AS publish
RUN dotnet publish KernelMind.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# Stage 3: Development (for docker-compose.override.yml)
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development
WORKDIR /app

# Install debugging tools
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Set environment for development
ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080

# Expose ports
EXPOSE 8080 9229

# Default command (will be overridden by docker-compose)
CMD ["dotnet", "watch", "run", "--urls", "http://+:8080"]

# ============================================
# Stage 4: Production
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS production
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish /app/publish .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "KernelMind.Api.dll"]
