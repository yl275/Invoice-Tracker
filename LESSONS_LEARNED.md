# 🧠 Lessons Learned & Troubleshooting Guide

This document records the technical challenges encountered during the Dockerization and setup of the InvoiceSystem project, along with their solutions.

## 1. Docker Database Startup Race Condition

### 🔴 The Issue
When starting the application via Docker Compose, the API container would crash or fail to connect to the database with the error:
`57P03: the database system is starting up`

### 💡 The Cause
`depends_on` in `docker-compose.yaml` only waits for the database container *to start*, not for the PostgreSQL service *to be ready* to accept connections. The API attempted to connect while Postgres was still initializing.

### ✅ The Solution
Implemented a Docker **Healthcheck** mechanism:
1.  **Database Service**: Added a `healthcheck` that runs `pg_isready` to report when the DB is truly ready.
2.  **API Service**: Updated `depends_on` to wait for the specific condition `service_healthy`.

```yaml
# docker-compose.yaml snippet
services:
  db:
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U admin -d InvoiceSystem"]
      interval: 5s
      timeout: 5s
      retries: 5
  api:
    depends_on:
      db:
        condition: service_healthy
```

---

## 2. EF Core Migrations vs. EnsureCreated()

### 🔴 The Issue
After switching from `context.Database.EnsureCreated()` to `context.Database.Migrate()`, the seeding process failed with:
`Table 'Clients' already exists` (or similar errors indicating schema conflicts).

### 💡 The Cause
*   `EnsureCreated()` creates tables directly but **does not** create the `__EFMigrationsHistory` table.
*   `Migrate()` checks `__EFMigrationsHistory`. If it's missing, it assumes the DB is empty and tries to run all migrations (creating tables again), leading to a conflict with the existing tables created by the previous method.

### ✅ The Solution
*   Switched code to strictly use `context.Database.Migrate()` in `DbInitializer.cs` to support proper schema evolution.
*   **Action Required**: Manually delete the old database volume (`docker-compose down -v`) so `Migrate()` can build the schema from scratch correctly.

---

## 3. PostgreSQL DateTime UTC Compatibility (Npgsql)

### 🔴 The Issue
During seeding, the application threw the following exception:
`System.ArgumentException: Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone'`

### 💡 The Cause
PostgreSQL is strict about time zones. The default logical type `timestamp without time zone` does not accept .NET `DateTime` with `Kind=Utc`. Newer versions of the Npgsql driver enforce this strictly to prevent timezone ambiguity.

### ✅ The Solution
Enabled the **Legacy Timestamp Behavior** switch in `Program.cs` to allow Npgsql to handle UTC dates automatically as it did in older versions.

```csharp
// Program.cs
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```
*(Alternative strict solution: Change all Entity properties to `DateTimeOffset` or Configure column types explicitly as `timestamp with time zone`, but the switch is the quickest fix for existing codebases).*
