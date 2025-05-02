#!/bin/bash
echo "Instalando dotnet ef..."
dotnet tool install --global dotnet-ef
echo "Aplicando migrations..."
dotnet ef database update --connection "Host=db;Port=5430;Database=${POSTGRES_DB};User ID=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};" --startup-project /src/Testezin.csproj --project /src/Testezin.csproj
echo "Migrations aplicadas."