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
4. Access API Documentation at: `http://localhost:5207/scalar/v1`

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

This project follows the **Clean Architecture** layering:

```
InvoiceSystem/
├── InvoiceSystem.API/              # [Web API Layer] - Entry point, contains Controllers and Program.cs
├── InvoiceSystem.Application/      # [Application Layer] - Core business logic, Service implementations, DTOs, and Interfaces
├── InvoiceSystem.Domain/           # [Domain Layer] - Core Entities and business rules, no dependencies
├── InvoiceSystem.Infrastructure/   # [Infrastructure Layer] - DbContext, Repository implementations
├── InvoiceSystem.UnitTests/        # [Unit Tests] - Tests for Service and Domain logic
├── InvoiceSystem.IntegrationTests/ # [Integration Tests] - End-to-end tests for API endpoints
└── InvoiceSystem.slnx              # Solution file
```
