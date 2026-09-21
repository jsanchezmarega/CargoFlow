@echo off

if "%1"=="db-up" goto db-up
if "%1"=="db-down" goto db-down
if "%1"=="db-client" goto db-client

echo Usage:
echo   dev.cmd db-up
echo   dev.cmd db-down
echo   dev.cmd db-client
goto :eof

:db-up
docker compose up -d
goto :eof

:db-down
docker compose down
goto :eof

:db-client
docker compose exec sqlserver /bin/bash -c "/opt/mssql-tools18/bin/sqlcmd -S \"$DB_HOST\" -U \"$DB_USER\" -P \"$DB_PASSWORD\" -d \"$DB_NAME\" -C"
goto :eof
