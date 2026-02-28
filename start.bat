@echo off
echo ==========================================
echo   InvoiceSystem - Docker Startup Script
echo ==========================================
echo.
echo Stopping any existing containers...
docker-compose down

echo.
echo Building and starting containers...
docker-compose up -d --build

echo.
echo ==========================================
echo   Application Started!
echo ==========================================
echo.
echo Web UI:        http://localhost
echo API docs:      http://localhost:5207/scalar/v1
echo.
echo You can close this window.
pause
