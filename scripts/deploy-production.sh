#!/bin/bash

# Deploy to Production Environment Script
# This script deploys the Jackson Ideas Mock application to production environment

set -e

echo "🚀 Starting production deployment..."

# Configuration
PRODUCTION_ENV="Production"
PRODUCTION_PORT="8080"
COMPOSE_FILE="docker-compose.production.yml"
APP_NAME="jackson-ideas-mock"
HEALTH_CHECK_URL="http://localhost:${PRODUCTION_PORT}/health"
BACKUP_DIR="./backups/$(date +%Y%m%d_%H%M%S)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

echo_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

echo_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check prerequisites
echo_info "Checking prerequisites..."

if ! command -v docker &> /dev/null; then
    echo_error "Docker is not installed or not in PATH"
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo_error "Docker Compose is not installed or not in PATH"
    exit 1
fi

# Confirmation for production deployment
echo_warn "⚠️  This will deploy to PRODUCTION environment!"
read -p "Are you sure you want to continue? (yes/no): " -r
if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo_info "Deployment cancelled."
    exit 0
fi

# Create backup directory
echo_info "Creating backup directory..."
mkdir -p "$BACKUP_DIR"

# Backup current deployment if exists
if docker-compose -f "$COMPOSE_FILE" ps | grep -q "Up"; then
    echo_info "Creating backup of current deployment..."
    docker-compose -f "$COMPOSE_FILE" logs > "$BACKUP_DIR/application.log"
    
    # Backup data volumes if they exist
    if docker volume ls | grep -q "${APP_NAME}_data"; then
        echo_info "Backing up data volumes..."
        docker run --rm -v "${APP_NAME}_data:/data" -v "$(pwd)/$BACKUP_DIR:/backup" alpine tar czf /backup/data_backup.tar.gz -C /data .
    fi
fi

# Build the application
echo_info "Building application..."
dotnet build --configuration Release --no-restore

if [ $? -ne 0 ]; then
    echo_error "Build failed"
    exit 1
fi

# Run comprehensive tests
echo_info "Running comprehensive test suite..."
dotnet test --configuration Release --no-build --verbosity minimal --collect:"XPlat Code Coverage"

if [ $? -ne 0 ]; then
    echo_error "Tests failed - cannot deploy to production"
    exit 1
fi

# Create production compose file if it doesn't exist
if [ ! -f "$COMPOSE_FILE" ]; then
    echo_info "Creating production compose file..."
    cat > "$COMPOSE_FILE" << EOL
version: '3.8'

services:
  mock-app:
    build:
      context: .
      dockerfile: Dockerfile.mock
    ports:
      - "${PRODUCTION_PORT}:10000"
    environment:
      - ASPNETCORE_ENVIRONMENT=${PRODUCTION_ENV}
      - ASPNETCORE_URLS=http://+:10000
      - MockConfiguration__EnableDemoMode=true
      - MockConfiguration__SimulateProcessingDelays=false
      - MockConfiguration__UseStaticData=true
      - MockConfiguration__EnableDetailedLogging=false
      - MockConfiguration__MaxConcurrentOperations=10
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:10000/health"]
      interval: 30s
      timeout: 10s
      retries: 5
      start_period: 60s
    volumes:
      - ./logs:/app/logs
      - prod_data:/app/data
    networks:
      - production-network
    restart: unless-stopped
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl:/etc/nginx/ssl:ro
    depends_on:
      - mock-app
    networks:
      - production-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost"]
      interval: 30s
      timeout: 10s
      retries: 3

networks:
  production-network:
    driver: bridge

volumes:
  prod_data:
    driver: local
EOL
fi

# Stop existing containers gracefully
echo_info "Stopping existing containers..."
docker-compose -f "$COMPOSE_FILE" down --timeout 30

# Pull latest base images
echo_info "Pulling latest base images..."
docker-compose -f "$COMPOSE_FILE" pull --ignore-pull-failures

# Build with no cache for production
echo_info "Building production images..."
docker-compose -f "$COMPOSE_FILE" build --no-cache

# Start the application
echo_info "Starting production environment..."
docker-compose -f "$COMPOSE_FILE" up -d

# Wait for application to be ready with extended timeout for production
echo_info "Waiting for application to be ready..."
RETRY_COUNT=0
MAX_RETRIES=60

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -f -s "$HEALTH_CHECK_URL" > /dev/null; then
        echo_info "Application is ready!"
        break
    fi
    
    echo_warn "Application not ready yet, retrying in 10 seconds... (${RETRY_COUNT}/${MAX_RETRIES})"
    sleep 10
    RETRY_COUNT=$((RETRY_COUNT + 1))
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo_error "Application failed to start within expected time"
    echo_info "Showing container logs:"
    docker-compose -f "$COMPOSE_FILE" logs --tail=100
    
    # Rollback option
    echo_warn "Would you like to rollback to previous version? (yes/no)"
    read -p "Rollback: " -r
    if [[ $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
        echo_info "Rolling back..."
        docker-compose -f "$COMPOSE_FILE" down
        # Restore from backup if available
        if [ -f "$BACKUP_DIR/data_backup.tar.gz" ]; then
            docker run --rm -v "${APP_NAME}_data:/data" -v "$(pwd)/$BACKUP_DIR:/backup" alpine tar xzf /backup/data_backup.tar.gz -C /data
        fi
    fi
    exit 1
fi

# Comprehensive deployment verification
echo_info "Running comprehensive deployment verification..."

# Test health endpoints
echo_info "Testing health endpoints..."
curl -f -s "$HEALTH_CHECK_URL" > /dev/null || { echo_error "Basic health check failed"; exit 1; }
curl -f -s "$HEALTH_CHECK_URL/detailed" > /dev/null || { echo_error "Detailed health check failed"; exit 1; }
curl -f -s "$HEALTH_CHECK_URL/ready" > /dev/null || { echo_error "Readiness check failed"; exit 1; }
curl -f -s "$HEALTH_CHECK_URL/live" > /dev/null || { echo_error "Liveness check failed"; exit 1; }

# Test main application endpoint
echo_info "Testing main application endpoint..."
MAIN_RESPONSE=$(curl -s -w "%{http_code}" "http://localhost:${PRODUCTION_PORT}" -o /dev/null)
if [ "$MAIN_RESPONSE" != "200" ]; then
    echo_error "Main application endpoint returned HTTP $MAIN_RESPONSE"
    exit 1
fi

echo_info "All verification checks passed!"

# Show deployment status
echo_info "Production deployment completed successfully!"
echo_info "Application URL: http://localhost:${PRODUCTION_PORT}"
echo_info "Health Check URL: ${HEALTH_CHECK_URL}"
echo_info "Backup created at: $BACKUP_DIR"

# Show running containers
echo_info "Running containers:"
docker-compose -f "$COMPOSE_FILE" ps

# Show resource usage
echo_info "Resource usage:"
docker stats --no-stream

echo_info "✅ Production deployment completed successfully!"
echo_info "Monitor logs with: docker-compose -f ${COMPOSE_FILE} logs -f"
echo_info "Monitor metrics at: ${HEALTH_CHECK_URL}/metrics"
echo_info "Stop deployment with: docker-compose -f ${COMPOSE_FILE} down"

# Log deployment to file
echo "$(date): Production deployment completed successfully" >> ./logs/deployment.log