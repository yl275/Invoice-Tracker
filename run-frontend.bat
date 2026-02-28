@echo off
cd /d "%~dp0InvoiceSystem.Frontend"
if not exist node_modules npm install
npm run dev
