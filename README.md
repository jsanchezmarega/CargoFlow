# CargoFlow

CargoFlow is a .NET logistics application for managing customers and shipments through their lifecycle.

The project is primarily a learning and demonstration application for exploring .NET application architecture, Entity Framework Core, SQL Server, WinForms, DevExpress, dependency injection, domain modeling, and automated testing.

## Tech Stack

- .NET 10
- C#
- Entity Framework Core
- SQL Server
- Docker / Docker Compose
- WinForms
- DevExpress WinForms
- xUnit
- Moq

## Projects

The solution is split into several projects:

- **CargoFlow** — Domain model, application services, persistence, and Entity Framework Core migrations.
- **CargoFlow.Desktop** — WinForms desktop application using DevExpress.
- **CargoFlow.Tests** — Domain and service tests.
- **CargoFlow.DevTools** — Development utilities such as database seeding.

## Prerequisites

To run the project locally, install:

- .NET 10 SDK
- Docker Desktop
- Entity Framework Core CLI tools
- Visual Studio with .NET desktop development support

The desktop application currently uses DevExpress WinForms controls and therefore also requires an appropriate DevExpress installation/license.

Verify the EF Core CLI with:

```bash
dotnet ef --version
```

If it is not installed:

```bash
dotnet tool install --global dotnet-ef
```

## Initial Setup

From the repository root:

```bat
dev.cmd setup
```

Setup will:

1. Create `.env` from `.env.example` if necessary.
2. Configure the repository's Git hooks.
3. Check that the configured SQL Server port is available.
4. Start SQL Server with Docker Compose.
5. Wait until SQL Server is ready.
6. Apply Entity Framework migrations.
7. Seed the development database.

If the configured SQL Server port is already occupied, setup will stop. Change `DB_PORT` in `.env` to an available port and run setup again.

Existing `.env` files are never overwritten.

## Development Commands

### Build

```bat
dev.cmd build
```

Builds the solution.

### Tests

```bat
dev.cmd test
```

Runs the automated test suite.

### Database

Start SQL Server:

```bat
dev.cmd db-up
```

Stop SQL Server:

```bat
dev.cmd db-down
```

Apply pending database migrations:

```bat
dev.cmd db-update
```

Seed the development database:

```bat
dev.cmd db-seed
```

Reset the database to a known development state:

```bat
dev.cmd db-reset
```

This drops the existing database, reapplies all migrations, and runs the seed process.

Connect to SQL Server using `sqlcmd`:

```bat
dev.cmd db-client
```

## Entity Framework Migrations

After changing the EF Core model, create a migration with:

```bat
dev.cmd migration-add <MigrationName>
```

For example:

```bat
dev.cmd migration-add AddShipmentTrackingNumber
```

The command checks whether the EF Core model has changed before creating the migration, preventing accidental empty migrations.

Apply migrations with:

```bat
dev.cmd db-update
```

## Tests

The test suite includes:

- Shipment domain lifecycle tests
- Valid and invalid shipment state transitions
- Service persistence tests using SQLite
- Notification interaction tests using Moq
- Customer service tests
- EF Core relationship/loading tests

Run all tests with:

```bat
dev.cmd test
```

## Shipment Lifecycle

Shipments currently support the following states:

```text
Planned
  ├──> InTransit ──> Delivered
  │        │
  │        └──> Cancelled
  │
  └──> Cancelled
```

Invalid state transitions raise an `InvalidShipmentStateException`.

## Development Database

Local SQL Server runs in Docker and persists its data in a Docker volume.

Development seed data can be recreated at any time with:

```bat
dev.cmd db-reset
```

The seed process creates customers and shipments in several lifecycle states so that the desktop UI can be exercised without manually creating data.