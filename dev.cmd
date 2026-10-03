@echo off

if "%1"=="setup" goto setup
if "%1"=="env" goto env
if "%1"=="hooks-install" goto hooks-install

if "%1"=="build" goto build
if "%1"=="test" goto test
if "%1"=="test-core" goto test-core
if "%1"=="test-api" goto test-api
if "%1"=="api" goto api

if "%1"=="db-up" goto db-up
if "%1"=="db-down" goto db-down
if "%1"=="db-wait" goto db-wait
if "%1"=="db-preflight" goto db-preflight
if "%1"=="db-check-port" goto db-check-port
if "%1"=="db-client" goto db-client
if "%1"=="db-update" goto db-update
if "%1"=="db-seed" goto db-seed
if "%1"=="db-reset" goto db-reset

if "%1"=="migration-add" goto migration-add

goto usage


:setup
echo Setting up CargoFlow...
echo.

call "%~f0" env
if errorlevel 1 exit /b %errorlevel%

call "%~f0" hooks-install
if errorlevel 1 exit /b %errorlevel%

call "%~f0" db-preflight
if errorlevel 1 exit /b %errorlevel%

call "%~f0" db-up
if errorlevel 1 exit /b %errorlevel%

call "%~f0" db-wait
if errorlevel 1 exit /b %errorlevel%

call "%~f0" db-update
if errorlevel 1 exit /b %errorlevel%

call "%~f0" db-seed
if errorlevel 1 exit /b %errorlevel%

echo.
echo CargoFlow setup complete.
exit /b 0


:env
if exist ".env" (
    echo .env already exists.
    exit /b 0
)

if not exist ".env.example" (
    echo ERROR: .env.example was not found.
    exit /b 1
)

copy ".env.example" ".env" >nul

if errorlevel 1 (
    echo ERROR: Could not create .env.
    exit /b 1
)

echo Created .env from .env.example.
exit /b 0


:hooks-install
if not exist ".githooks\pre-commit" (
    echo ERROR: .githooks\pre-commit was not found.
    exit /b 1
)

git config core.hooksPath .githooks

if errorlevel 1 (
    echo ERROR: Could not configure Git hooks.
    exit /b 1
)

echo Git hooks configured.
exit /b 0


:build
dotnet build
exit /b %errorlevel%


:test
dotnet test
exit /b %errorlevel%


:test-core
dotnet test CargoFlow.Tests\CargoFlow.Tests.csproj
exit /b %errorlevel%


:test-api
dotnet test CargoFlow.Api.Tests\CargoFlow.Api.Tests.csproj
exit /b %errorlevel%


:api
dotnet watch --project CargoFlow.Api\CargoFlow.Api.csproj
exit /b %errorlevel%


:db-up
docker compose up -d
exit /b %errorlevel%


:db-down
docker compose down
exit /b %errorlevel%


:db-wait
echo Waiting for SQL Server...

for /L %%i in (1,1,30) do (
    for /f "delims=" %%s in ('docker inspect --format="{{.State.Health.Status}}" cargoflow-sqlserver 2^>nul') do (
        if "%%s"=="healthy" (
            echo SQL Server is ready.
            exit /b 0
        )
    )

    timeout /t 2 /nobreak >nul
)

echo ERROR: SQL Server did not become healthy in time.
exit /b 1


:db-preflight
docker compose ps -q sqlserver | findstr . >nul

if not errorlevel 1 (
    echo CargoFlow SQL Server container is already running.
    exit /b 0
)

call "%~f0" db-check-port
exit /b %errorlevel%


:db-check-port
set "DB_PORT="

for /f "tokens=2 delims==" %%p in ('findstr /B "DB_PORT=" .env') do set "DB_PORT=%%p"

if not defined DB_PORT (
    echo ERROR: DB_PORT is not defined in .env.
    exit /b 1
)

echo Checking port %DB_PORT%...

powershell -NoProfile -Command ^
    "$port = %DB_PORT%; " ^
    "$listener = Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue; " ^
    "if ($listener) { exit 1 } else { exit 0 }"

if errorlevel 1 (
    echo ERROR: Port %DB_PORT% is already in use.
    echo.
    echo Change DB_PORT in .env to an available port and run setup again.
    exit /b 1
)

echo Port %DB_PORT% is available.
exit /b 0


:db-client
docker compose exec sqlserver /bin/bash -c "/opt/mssql-tools18/bin/sqlcmd -S \"$DB_HOST\" -U \"$DB_USER\" -P \"$DB_PASSWORD\" -d \"$DB_NAME\" -C"
exit /b %errorlevel%


:db-update
dotnet ef database update --project CargoFlow\CargoFlow.csproj
exit /b %errorlevel%


:db-seed
dotnet run --project CargoFlow.DevTools\CargoFlow.DevTools.csproj -- seed
exit /b %errorlevel%


:db-reset
dotnet ef database drop --project CargoFlow\CargoFlow.csproj --force
if errorlevel 1 exit /b %errorlevel%

dotnet ef database update --project CargoFlow\CargoFlow.csproj
if errorlevel 1 exit /b %errorlevel%

dotnet run --project CargoFlow.DevTools\CargoFlow.DevTools.csproj -- seed
if errorlevel 1 exit /b %errorlevel%

echo.
echo Database reset complete.
exit /b 0


:migration-add
if "%~2"=="" (
    echo ERROR: Missing migration name.
    echo Usage: dev.cmd migration-add ^<name^>
    exit /b 1
)

dotnet ef migrations has-pending-model-changes --project CargoFlow\CargoFlow.csproj >nul 2>&1

if not errorlevel 1 (
    echo No model changes detected. Migration was not created.
    exit /b 1
)

dotnet ef migrations add "%~2" --project CargoFlow\CargoFlow.csproj
exit /b %errorlevel%


:usage
echo CargoFlow development commands
echo.
echo Usage:
echo   dev.cmd setup
echo   dev.cmd env
echo   dev.cmd hooks-install
echo.
echo   dev.cmd build
echo   dev.cmd test
echo   dev.cmd test-core
echo   dev.cmd test-api
echo   dev.cmd api
echo.
echo   dev.cmd db-up
echo   dev.cmd db-down
echo   dev.cmd db-wait
echo   dev.cmd db-preflight
echo   dev.cmd db-check-port
echo   dev.cmd db-client
echo   dev.cmd db-update
echo   dev.cmd db-seed
echo   dev.cmd db-reset
echo.
echo   dev.cmd migration-add ^<name^>
exit /b 1
