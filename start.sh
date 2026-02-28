#!/bin/bash

echo "=========================================="
echo "  InvoiceSystem - Docker Startup Script"
echo "=========================================="
echo ""
echo "Stopping any existing containers..."
docker-compose down

echo ""
echo "Building and starting containers..."
docker-compose up -d --build

echo ""
echo "=========================================="
echo "  Application Started!"
echo "=========================================="
echo ""
echo "API Documentation available at: http://localhost:5207/scalar/v1"
echo ""
