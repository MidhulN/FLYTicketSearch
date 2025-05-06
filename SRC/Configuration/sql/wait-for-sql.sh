#!/bin/bash

echo "Waiting for SQL Server to be ready..."

until /opt/mssql-tools/bin/sqlcmd -S fly.flight.sqlserver -U sa -P Flyflight!1 -Q "SELECT name FROM sys.databases WHERE name = 'flights'" | grep -q flights; do
  >&2 echo "Database is not ready - sleeping"
  sleep 5
done

echo "Database is ready - starting application"
exec "$@"
