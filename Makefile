# CargoFlow development commands for macOS and Linux.
# Run from the repository root alongside docker-compose.yml.
.DEFAULT_GOAL := help
SHELL := /bin/sh

.PHONY: help setup env hooks-install preflight db-check-port build test test-core test-api test-blazor start stop api blazor restart-api restart-blazor db-up db-down db-wait db-client db-update db-seed db-reset migration-add

help:
	@printf '%s\n' 'CargoFlow development commands:' '  make setup       First-time setup (Docker + migrations + seed)' '  make start       Run SQL Server, API, and Blazor' '  make stop        Stop containers (keep database volume)' '  make api         Run API and dependencies' '  make blazor      Run Blazor and dependencies' '  make build       Build API and Blazor (cross-platform)' '  make test        Run tests (core, API, Blazor)' '  make test-core | test-api | test-blazor' '  make db-up | db-down | db-wait | db-update | db-seed | db-reset | db-client' '  make migration-add NAME=MigrationName'

env:
	@if [ -f .env ]; then echo '.env already exists.'; \
	elif [ -f .env.example ]; then cp .env.example .env && echo 'Created .env from .env.example.'; \
	else echo 'ERROR: .env.example was not found.' >&2; exit 1; fi

hooks-install:
	@test -f .githooks/pre-commit || { echo 'ERROR: .githooks/pre-commit missing.' >&2; exit 1; }
	@git config core.hooksPath .githooks
	@echo 'Git hooks configured.'

# Keep the port check host-specific, and skip it when the Compose DB is already running.
db-check-port:
	@port=$$(sed -n 's/^DB_PORT=//p' .env | tail -n 1); \
	case "$$port" in ''|*[!0-9]*) echo 'ERROR: DB_PORT must be numeric in .env.' >&2; exit 1;; esac; \
	if command -v lsof >/dev/null 2>&1; then \
	  if lsof -nP -iTCP:$$port -sTCP:LISTEN 2>/dev/null | grep -q .; then \
	    echo "ERROR: Port $$port is already in use. Change DB_PORT in .env." >&2; exit 1; \
	  fi; \
	else echo 'WARNING: lsof unavailable; skipping host port check.'; fi

preflight:
	@docker info >/dev/null 2>&1 || { echo 'ERROR: Docker is not running.' >&2; exit 1; }
	@docker compose version >/dev/null 2>&1 || { echo 'ERROR: Docker Compose is unavailable.' >&2; exit 1; }
	@if [ -z "$$(docker compose ps -q sqlserver 2>/dev/null)" ]; then $(MAKE) db-check-port; \
	else echo 'SQL Server container already exists; skipping port preflight.'; fi

setup: env hooks-install preflight
	@$(MAKE) db-up
	@$(MAKE) db-wait
	@$(MAKE) db-update
	@$(MAKE) db-seed
	@echo 'CargoFlow setup complete.'

# The Windows-only WinForms project is intentionally excluded from macOS builds.
build:
	dotnet build CargoFlow.Api/CargoFlow.Api.csproj
	dotnet build CargoFlow.Blazor/CargoFlow.Blazor.csproj

test: test-core test-api test-blazor

test-core:
	dotnet test CargoFlow.Tests/CargoFlow.Tests.csproj

test-api:
	dotnet test CargoFlow.Api.Tests/CargoFlow.Api.Tests.csproj

test-blazor:
	dotnet test CargoFlow.Blazor.Tests/CargoFlow.Blazor.Tests.csproj

start:
	docker compose up

stop:
	docker compose down

api:
	docker compose up api

blazor:
	docker compose up blazor

restart-api:
	docker compose restart api

restart-blazor:
	docker compose restart blazor

db-up:
	docker compose up -d sqlserver

db-down:
	docker compose stop sqlserver

db-wait:
	@echo 'Waiting for SQL Server to become healthy...'
	@i=0; while [ $$i -lt 60 ]; do \
	  status=$$(docker inspect --format='{{.State.Health.Status}}' cargoflow-sqlserver 2>/dev/null || true); \
	  if [ "$$status" = healthy ]; then echo 'SQL Server is ready.'; exit 0; fi; \
	  if [ "$$status" = unhealthy ]; then echo 'ERROR: SQL Server is unhealthy.' >&2; exit 1; fi; \
	  i=$$((i + 1)); sleep 2; \
	done; echo 'ERROR: SQL Server did not become healthy in 120 seconds.' >&2; exit 1

db-client:
	docker compose exec sqlserver /bin/bash -c '/opt/mssql-tools18/bin/sqlcmd -S localhost -U "$$DB_USER" -P "$$DB_PASSWORD" -d "$$DB_NAME" -C'

db-update:
	dotnet ef database update --project CargoFlow/CargoFlow.csproj

db-seed:
	dotnet run --project CargoFlow.DevTools/CargoFlow.DevTools.csproj -- seed

db-reset:
	dotnet ef database drop --project CargoFlow/CargoFlow.csproj --force
	$(MAKE) db-update
	$(MAKE) db-seed

migration-add:
	@test -n "$(NAME)" || { echo 'Usage: make migration-add NAME=MigrationName' >&2; exit 1; }
	@dotnet ef migrations has-pending-model-changes --project CargoFlow/CargoFlow.csproj >/dev/null 2>&1 && { echo 'No model changes detected.' >&2; exit 1; } || true
	dotnet ef migrations add "$(NAME)" --project CargoFlow/CargoFlow.csproj
