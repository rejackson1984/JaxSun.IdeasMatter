#!/bin/bash

# Environment Configuration Validation Script
# This script validates environment-specific configurations

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

echo_section() {
    echo -e "${BLUE}[SECTION]${NC} $1"
}

# Configuration
ENVIRONMENTS=("Development" "Staging" "Production")
CONFIG_DIR="/mnt/c/Development/Jackson.Ideas/src/Jackson.Ideas.Mock"
VALIDATION_ERRORS=0

echo_section "🔍 Starting Environment Configuration Validation"

# Function to validate JSON syntax
validate_json() {
    local file=$1
    local env=$2
    
    if [ ! -f "$file" ]; then
        echo_error "Configuration file not found: $file"
        VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
        return 1
    fi
    
    if ! python3 -m json.tool "$file" > /dev/null 2>&1; then
        echo_error "Invalid JSON syntax in $env configuration: $file"
        VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
        return 1
    fi
    
    echo_info "$env configuration JSON syntax is valid"
    return 0
}

# Function to validate required configuration sections
validate_config_sections() {
    local file=$1
    local env=$2
    
    # Required sections
    local required_sections=("Logging" "MockConfiguration" "HealthChecks" "Performance" "Security")
    
    for section in "${required_sections[@]}"; do
        if ! grep -q "\"$section\"" "$file"; then
            echo_error "$env configuration missing required section: $section"
            VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
        else
            echo_info "$env configuration has required section: $section"
        fi
    done
}

# Function to validate environment-specific settings
validate_environment_settings() {
    local file=$1
    local env=$2
    
    case $env in
        "Development")
            # Development should have debug logging
            if ! grep -q "\"Default\": \"Debug\"" "$file"; then
                echo_warn "Development should typically use Debug logging level"
            fi
            
            # Development should allow detailed error responses
            if ! grep -q "\"EnableDetailedErrorResponses\": true" "$file"; then
                echo_warn "Development should typically enable detailed error responses"
            fi
            ;;
            
        "Staging")
            # Staging should have info logging
            if ! grep -q "\"Default\": \"Information\"" "$file"; then
                echo_warn "Staging should typically use Information logging level"
            fi
            
            # Staging should have detailed logging enabled
            if ! grep -q "\"EnableDetailedLogging\": true" "$file"; then
                echo_warn "Staging should typically enable detailed logging for debugging"
            fi
            ;;
            
        "Production")
            # Production should not have debug logging
            if grep -q "\"Default\": \"Debug\"" "$file"; then
                echo_error "Production should not use Debug logging level"
                VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
            fi
            
            # Production should not show detailed errors
            if grep -q "\"EnableDetailedErrorResponses\": true" "$file"; then
                echo_error "Production should not enable detailed error responses for security"
                VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
            fi
            
            # Production should have GC optimization
            if ! grep -q "\"EnableGCOptimization\": true" "$file"; then
                echo_warn "Production should typically enable GC optimization"
            fi
            ;;
    esac
}

# Function to validate security settings
validate_security_settings() {
    local file=$1
    local env=$2
    
    # Check for rate limiting in production/staging
    if [[ "$env" == "Production" || "$env" == "Staging" ]]; then
        if ! grep -q "\"EnableRequestRateLimiting\": true" "$file"; then
            echo_warn "$env should typically enable request rate limiting"
        fi
    fi
    
    # Check for reasonable memory limits
    if grep -q "MaxMemoryUsageMB" "$file"; then
        memory_limit=$(grep -o "\"MaxMemoryUsageMB\": [0-9]*" "$file" | grep -o "[0-9]*")
        if [ "$memory_limit" -lt 64 ]; then
            echo_warn "$env memory limit seems very low: ${memory_limit}MB"
        elif [ "$memory_limit" -gt 2048 ]; then
            echo_warn "$env memory limit seems very high: ${memory_limit}MB"
        fi
    fi
}

# Main validation loop
for env in "${ENVIRONMENTS[@]}"; do
    echo_section "Validating $env Environment"
    
    config_file="$CONFIG_DIR/appsettings.$env.json"
    
    # Validate JSON syntax
    if validate_json "$config_file" "$env"; then
        # Validate configuration sections
        validate_config_sections "$config_file" "$env"
        
        # Validate environment-specific settings
        validate_environment_settings "$config_file" "$env"
        
        # Validate security settings
        validate_security_settings "$config_file" "$env"
    fi
    
    echo ""
done

# Validate Docker configurations
echo_section "Validating Docker Configurations"

# Check if Dockerfile exists
if [ -f "$CONFIG_DIR/../../Dockerfile.mock" ]; then
    echo_info "Dockerfile.mock exists"
    
    # Check for multi-stage build
    if grep -q "FROM.*AS.*" "$CONFIG_DIR/../../Dockerfile.mock"; then
        echo_info "Dockerfile uses multi-stage build"
    else
        echo_warn "Dockerfile should use multi-stage build for optimization"
    fi
else
    echo_error "Dockerfile.mock not found"
    VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
fi

# Check if docker-compose files exist
compose_files=("docker-compose.yml" "docker-compose.staging.yml" "docker-compose.production.yml")
for compose_file in "${compose_files[@]}"; do
    if [ -f "$CONFIG_DIR/../../$compose_file" ]; then
        echo_info "$compose_file exists"
    else
        echo_warn "$compose_file not found (may be created during deployment)"
    fi
done

# Validate deployment scripts
echo_section "Validating Deployment Scripts"

deployment_scripts=("scripts/deploy-staging.sh" "scripts/deploy-production.sh")
for script in "${deployment_scripts[@]}"; do
    script_path="$CONFIG_DIR/../../$script"
    if [ -f "$script_path" ]; then
        echo_info "$script exists"
        
        # Check if executable
        if [ -x "$script_path" ]; then
            echo_info "$script is executable"
        else
            echo_warn "$script is not executable"
        fi
    else
        echo_error "$script not found"
        VALIDATION_ERRORS=$((VALIDATION_ERRORS + 1))
    fi
done

# Summary
echo_section "Validation Summary"

if [ $VALIDATION_ERRORS -eq 0 ]; then
    echo_info "✅ All environment configurations are valid!"
    echo_info "Environment configurations are ready for deployment"
    exit 0
else
    echo_error "❌ Found $VALIDATION_ERRORS validation error(s)"
    echo_error "Please fix the configuration issues before deploying"
    exit 1
fi