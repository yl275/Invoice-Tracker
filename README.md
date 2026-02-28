# InvoiceSystem

## 📖 Project Summary

InvoiceSystem is a simple and efficient Web API application designed to manage invoices, clients, and products. It allows users to create and read invoice records, manage client information, and maintain a product catalog.

Key features include:
- **Invoice Management**: Create new invoices (with automatic total calculation), query invoice details and lists.
- **Client Management**: Register, update, and query client information.
- **Product Management**: Add, update, and query product information (supports SKU/inventory management).
- **Business Validation**: Built-in strict business rule validation (e.g., price cannot be negative, must include client information, etc.).

## 🛠 Tech Stack

This project is built using a modern .NET technology stack, following Clean Architecture principles.

- **Core Framework**: [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) (ASP.NET Core Web API)
- **ORM / Data Access**: Entity Framework Core 9.0
- **API Documentation**: Scalar / OpenAPI (Swagger)
- **Unit Testing**: xUnit, Moq, FluentAssertions
- **Integration Testing**: Microsoft.AspNetCore.Mvc.Testing (InMemory Database)
- **Web UI**: React 19, Vite 7, Tailwind CSS, React Router (in `InvoiceSystem.Frontend/`)

## 🚀 How to Run

### Prerequisites
- Install [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run with Docker (One-Click)
1. Ensure [Docker Desktop](https://www.docker.com/products/docker-desktop/) is installed and running.

**Windows:**
2. Double-click `start.bat` in the root directory.

**Mac / Linux:**
2. Open a terminal in the root directory.
3. Run `chmod +x start.sh` (first time only).
4. Run `./start.sh`.

3. Wait for the containers to start.
4. **Web UI**: open `http://localhost` in your browser.
5. **API docs**: `http://localhost:5207/scalar/v1`

### Manual Run (Development)
1. Clone the repository:
   ```bash
   git clone https://github.com/yl275/InvoiceSystem.git
   cd InvoiceSystem
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the API project:
   ```bash
   cd InvoiceSystem.API
   dotnet run
   ```

4. Access API Documentation:
   Once started, you can typically access the interactive API documentation at `http://localhost:5207/scalar/v1` (depending on your specific `launchSettings.json` configuration).

### Run Frontend (with API)

To use the web UI against the running API:

1. Start the API (Docker as above, or `cd InvoiceSystem.API && dotnet run`).
2. From the repo root, run the frontend:
   - **Windows**: double-click `run-frontend.bat` or run `cd InvoiceSystem.Frontend && npm install && npm run dev`.
   - **Mac/Linux**: `cd InvoiceSystem.Frontend && npm install && npm run dev`.
3. Open `http://localhost:5173` (Vite dev server; it proxies `/api` to the API).

**Prerequisites for frontend**: [Node.js](https://nodejs.org/) 20+ and npm.

## ☁️ Deploy to Render

[Render](https://render.com) supports Docker + PostgreSQL. Two options:

### Option A: Blueprint (recommended)

1. Push this repo to GitHub/GitLab/Bitbucket.
2. Open [Render Dashboard](https://dashboard.render.com/) → **New** → **Blueprint**.
3. Connect the repo; Render will detect `render.yaml` at the root.
4. Click **Deploy Blueprint**.
5. After deploy:
   - **API**: `https://invoicesystem-api-xxxx.onrender.com` (use `/health` for health checks).
   - **Web UI**: `https://invoicesystem-frontend-xxxx.onrender.com` (static site; it calls the API automatically).

### Option B: Manual setup

1. In Render: **New** → **PostgreSQL** → create a database (e.g. plan **Free**), note the **Internal Database URL**.
2. **New** → **Web Service** → connect the repo.
3. Set **Runtime** to **Docker** (use repo root Dockerfile).
4. Add **Environment Variables**:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ASPNETCORE_URLS` = `http://0.0.0.0:10000`
   - `ConnectionStrings__DefaultConnection` = *(paste the Internal Database URL from step 1)*
5. Deploy. The app applies EF Core migrations on startup.

## 🧪 How to Test

This project includes two types of tests: Unit Tests (for business logic) and Integration Tests (for API endpoints).

Run all tests from the solution root:

```bash
dotnet test
```

Or run them individually:
```bash
# Run Unit Tests
dotnet test InvoiceSystem.UnitTests

# Run Integration Tests
dotnet test InvoiceSystem.IntegrationTests
```

Or use Scaler to test:

Start the project, and go to http://localhost:5207/scalar/v1

## 📂 Directory Structure

Backend follows **Clean Architecture**; frontend is a React + Vite SPA.

```
InvoiceSystem/
├── InvoiceSystem.API/              # [Web API] - Controllers, Program.cs
├── InvoiceSystem.Application/      # [Application] - Services, DTOs, interfaces
├── InvoiceSystem.Domain/          # [Domain] - Entities and business rules
├── InvoiceSystem.Infrastructure/   # [Infrastructure] - DbContext, repositories
├── InvoiceSystem.Frontend/         # [Web UI] - React + Vite + Tailwind, calls /api
├── InvoiceSystem.UnitTests/
├── InvoiceSystem.IntegrationTests/
├── docker-compose.yaml             # API + Frontend (nginx) + PostgreSQL
├── render.yaml                     # Render Blueprint (API + DB only)
└── InvoiceSystem.slnx
```
