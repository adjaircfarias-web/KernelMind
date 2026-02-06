# KernelMind Web - Angular 19 Application
# Multi-stage build with Nginx for production

# ============================================
# Stage 1: Dependencies
# ============================================
FROM node:20-alpine AS dependencies
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production && npm cache clean --force

# ============================================
# Stage 2: Build
# ============================================
FROM node:20-alpine AS build
WORKDIR /app

# Copy dependencies from previous stage
COPY --from=dependencies /app/node_modules ./node_modules

# Copy source code
COPY . .

# Build for production
RUN npm run build -- --configuration production

# ============================================
# Stage 3: Development (for docker-compose.override.yml)
# ============================================
FROM node:20-alpine AS development
WORKDIR /app

# Install Angular CLI globally
RUN npm install -g @angular/cli@19

# Set environment
ENV NODE_ENV=development \
    CHOKIDAR_USEPOLLING=true

# Expose Angular dev server port
EXPOSE 4200

# Default command (will be overridden by docker-compose)
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4200", "--poll", "2000"]

# ============================================
# Stage 4: Production
# ============================================
FROM nginx:alpine AS production

# Install curl for healthcheck
RUN apk add --no-cache curl

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built application from build stage
COPY --from=build /app/dist/kernelmind-web/browser /usr/share/nginx/html

# Create nginx user
RUN adduser -D -H -u 101 -s /sbin/nologin nginxuser

# Set proper permissions
RUN chown -R nginxuser:nginxuser /usr/share/nginx/html && \
    chown -R nginxuser:nginxuser /var/cache/nginx && \
    chown -R nginxuser:nginxuser /var/log/nginx && \
    chown -R nginxuser:nginxuser /etc/nginx/conf.d

# Remove default nginx user directive
RUN sed -i 's/user  nginx;/user  nginxuser;/' /etc/nginx/nginx.conf

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:80 || exit 1

# Expose port
EXPOSE 80

# Run nginx
CMD ["nginx", "-g", "daemon off;"]
