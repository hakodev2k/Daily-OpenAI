$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj') -- inspect
exit $LASTEXITCODE
