# CargoFlow

CargoFlow is a logistics application for managing customers and shipments from planning through delivery or cancellation.

I built this project while learning and applying the C#/.NET technology stack used in professional logistics software. The goal is to transfer my existing backend development experience into the .NET ecosystem through a practical application rather than isolated tutorials or exercises.

The application currently includes:

- A logistics domain model with customers, shipments, and controlled shipment lifecycle transitions.
- A Windows desktop application for managing shipment data.
- A REST API for accessing and managing customers and shipments.
- A Blazor web application for viewing and filtering shipments.
- SQL Server persistence with database migrations and development seed data.
- Automated domain, service, persistence, and API integration tests.
- A Docker Compose development environment with hot reload for the API and Blazor applications.
- Development tooling for setup, database management, migrations, testing, and Git hooks.

The project is actively being developed as I expand my experience with the .NET ecosystem. Planned areas include further development of the Blazor interface, .NET MAUI, and exploration of the Microsoft Azure platform.

## Tech Stack

- .NET 10
- C#
- ASP.NET Core
- Blazor
- Entity Framework Core
- SQL Server
- SQLite for automated tests
- Docker / Docker Compose
- WinForms
- DevExpress WinForms
- xUnit
- Moq

## Projects

The solution is split into several projects:

- **CargoFlow** — Domain model, application services, persistence, and Entity Framework Core migrations.
- **CargoFlow.Api** — ASP.NET Core REST API for customers and shipments.
- **CargoFlow.Api.Tests** — HTTP integration tests for the ASP.NET Core API.
- **CargoFlow.Blazor** — Blazor web application consuming the REST API.
- **CargoFlow.Desktop** — WinForms desktop application using DevExpress.
- **CargoFlow.Tests** — Domain, service, and persistence tests.
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

The default development ports are configured in `.env`:

```text
DB_PORT=1433
API_PORT=5266
BLAZOR_PORT=5000
```

## Running CargoFlow

Start the complete development environment with:

```bat
dev.cmd start
```

Docker Compose starts:

- SQL Server
- CargoFlow API
- CargoFlow Blazor

The API and Blazor applications run using `dotnet watch`, so source changes are detected and applied during development.

With the default `.env` configuration:

```text
Blazor: http://localhost:5000
API:    http://localhost:5266
```

`start` does not apply migrations or seed the database. Run `dev.cmd setup` for initial setup or use the individual database commands when database maintenance is required.

Stop and remove the development containers and Compose network with:

```bat
dev.cmd stop
```

The persistent SQL Server data volume is retained.

Individual parts of the application can also be started through Docker Compose:

```bat
dev.cmd api
```

starts the API and its SQL Server dependency.

```bat
dev.cmd blazor
```

starts Blazor together with the API and SQL Server dependency chain.

## Development Commands

### Build

```bat
dev.cmd build
```

Builds the solution on the host.

### Tests

Run all automated tests:

```bat
dev.cmd test
```

Run only the core domain, service, and persistence tests:

```bat
dev.cmd test-core
```

Run only the API integration tests:

```bat
dev.cmd test-api
```

### Development Environment

Start the complete Docker Compose development environment:

```bat
dev.cmd start
```

Stop the development environment:

```bat
dev.cmd stop
```

Start the API and its dependencies:

```bat
dev.cmd api
```

Start the Blazor application and its dependencies:

```bat
dev.cmd blazor
```

### Database

Start only SQL Server in the background:

```bat
dev.cmd db-up
```

Stop SQL Server:

```bat
dev.cmd db-down
```

Wait until SQL Server reports a healthy state:

```bat
dev.cmd db-wait
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

Connect directly to the CargoFlow development database using `sqlcmd`:

```bat
dev.cmd db-client
```

## Web Application

`CargoFlow.Blazor` is a Blazor web application that consumes `CargoFlow.Api` over HTTP.

The shipments page currently supports:

- Retrieving shipments from the REST API.
- Filtering shipments by status.
- Filtering shipments by customer.
- Filtering shipments by origin.
- Loading customer data for filter selection.
- Loading and error states for API requests.

The Blazor application and API run as separate services in Docker Compose. Within the Docker network, Blazor communicates with the API using the Compose service name rather than a host port.

## REST API

CargoFlow exposes an ASP.NET Core REST API for customers and shipments.

### Customers

```text
GET  /api/customers
GET  /api/customers/{id}
POST /api/customers
```

### Shipments

```text
GET  /api/shipments
GET  /api/shipments/{id}
POST /api/shipments

POST /api/shipments/{id}/start-transit
POST /api/shipments/{id}/deliver
POST /api/shipments/{id}/cancel
```

The shipment collection endpoint supports optional filtering by status, customer, and origin.

Shipment lifecycle operations are explicit commands rather than direct status updates so that state transitions remain controlled by the domain model.

The API uses standard HTTP status codes including:

- `200 OK` for successful queries and lifecycle operations.
- `201 Created` when resources are created.
- `400 Bad Request` for invalid request data.
- `404 Not Found` when a requested resource does not exist.
- `409 Conflict` when a shipment lifecycle transition is invalid.

Invalid shipment state transitions are handled centrally and returned as HTTP Problem Details responses.

Example HTTP requests for development are available under:

```text
CargoFlow.Api/Http/
├── customers.http
└── shipments.http
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

## Automated Testing

CargoFlow has two automated test projects.

### Core Tests

`CargoFlow.Tests` covers:

- Shipment domain lifecycle behavior.
- Valid and invalid shipment state transitions.
- Customer and shipment service behavior.
- EF Core persistence using SQLite.
- Entity relationships and loading.
- Notification interactions using Moq.

Run them with:

```bat
dev.cmd test-core
```

### API Integration Tests

`CargoFlow.Api.Tests` exercises the ASP.NET Core application through HTTP using an in-memory test server and SQLite database.

The integration tests cover:

- Customer collection and individual-resource endpoints.
- Customer creation and request validation.
- Shipment retrieval, filtering, and creation.
- Shipment lifecycle operations.
- `404 Not Found` behavior.
- `409 Conflict` behavior for invalid lifecycle transitions.

Run them with:

```bat
dev.cmd test-api
```

Run the complete test suite with:

```bat
dev.cmd test
```

The repository also configures a pre-commit Git hook during setup that runs the automated tests before a commit is created.

## Shipment Lifecycle

Shipments support the following states:

```text
Planned
  ├──> InTransit ──> Delivered
  │        │
  │        └──> Cancelled
  │
  └──> Cancelled
```

Invalid state transitions raise an `InvalidShipmentStateException`.

The domain model owns these transition rules. The desktop application, application services, and REST API therefore use the same lifecycle behavior rather than implementing state rules independently.

## Development Database

Local SQL Server runs in Docker and persists its data in a Docker volume.

The API connects to SQL Server over the internal Docker Compose network, while host-side Entity Framework and development commands connect through the SQL Server port configured in `.env`.

Development seed data can be recreated at any time with:

```bat
dev.cmd db-reset
```

The seed process creates customers and shipments in several lifecycle states so that the desktop application, REST API, and Blazor interface can be exercised without manually creating data.