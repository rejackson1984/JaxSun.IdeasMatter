#!/bin/bash

# Deploy to Staging Environment Script
# This script deploys the Jackson Ideas Mock application to staging environment

set -e

echo "🚀 Starting staging deployment..."

# Configuration
STAGING_ENV="Staging"
STAGING_PORT="8080"
COMPOSE_FILE="docker-compose.staging.yml"
APP_NAME="jackson-ideas-mock"
HEALTH_CHECK_URL="http://localhost:${STAGING_PORT}/health"

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

# Build the application
echo_info "Building application..."
dotnet build --configuration Release --no-restore

if [ $? -ne 0 ]; then
    echo_error "Build failed"
    exit 1
fi

# Run tests
echo_info "Running tests..."
dotnet test --configuration Release --no-build --verbosity minimal

if [ $? -ne 0 ]; then
    echo_error "Tests failed"
    exit 1
fi

# Create staging compose file if it doesn't exist
if [ ! -f "$COMPOSE_FILE" ]; then
    echo_info "Creating staging compose file..."
    cat > "$COMPOSE_FILE" << EOL
version: '3.8'

services:
  mock-app:
    build:
      context: .
      dockerfile: Dockerfile.mock
    ports:
      - "${STAGING_PORT}:10000"
    environment:
      - ASPNETCORE_ENVIRONMENT=${STAGING_ENV}
      - ASPNETCORE_URLS=http://+:10000
      - MockConfiguration__EnableDemoMode=true
      - MockConfiguration__SimulateProcessingDelays=true
      - MockConfiguration__UseStaticData=true
      - MockConfiguration__EnableDetailedLogging=true
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:10000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    volumes:
      - ./logs:/app/logs
    networks:
      - staging-network

networks:
  staging-network:
    driver: bridge
EOL
fi

# Stop existing containers
echo_info "Stopping existing containers..."
docker-compose -f "$COMPOSE_FILE" down --remove-orphans

# Remove old images to ensure fresh deployment
echo_info "Removing old images..."
docker image prune -f
docker-compose -f "$COMPOSE_FILE" build --no-cache

# Start the application
echo_info "Starting staging environment..."
docker-compose -f "$COMPOSE_FILE" up -d

# Wait for application to be ready
echo_info "Waiting for application to be ready..."
RETRY_COUNT=0
MAX_RETRIES=30

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
    docker-compose -f "$COMPOSE_FILE" logs --tail=50
    exit 1
fi

# Verify deployment
echo_info "Verifying deployment..."
HEALTH_RESPONSE=$(curl -s "$HEALTH_CHECK_URL")
echo "Health check response: $HEALTH_RESPONSE"

# Show deployment status
echo_info "Deployment completed successfully!"
echo_info "Application URL: http://localhost:${STAGING_PORT}"
echo_info "Health Check URL: ${HEALTH_CHECK_URL}"
echo_info "Detailed Health Check: ${HEALTH_CHECK_URL}/detailed"

# Show running containers
echo_info "Running containers:"
docker-compose -f "$COMPOSE_FILE" ps

echo_info "✅ Staging deployment completed successfully!"
echo_info "Monitor logs with: docker-compose -f ${COMPOSE_FILE} logs -f"
echo_info "Stop deployment with: docker-compose -f ${COMPOSE_FILE} down"