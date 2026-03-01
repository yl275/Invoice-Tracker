@echo off
echo ==========================================
echo   InvoiceSystem - Docker Dev Startup
echo ==========================================
echo.
echo Stopping existing containers...
docker-compose -f docker-compose.yaml -f docker-compose.dev.yaml down

echo.
echo Building and starting DEV containers...
docker-compose -f docker-compose.yaml -f docker-compose.dev.yaml up -d --build

echo.
echo ==========================================
echo   Dev Environment Started!
echo ==========================================
echo.
echo Frontend (HMR): http://localhost:5173
echo API docs:       http://localhost:5207/scalar/v1
echo.
echo Use Ctrl+F5 in browser if cached.
pause
