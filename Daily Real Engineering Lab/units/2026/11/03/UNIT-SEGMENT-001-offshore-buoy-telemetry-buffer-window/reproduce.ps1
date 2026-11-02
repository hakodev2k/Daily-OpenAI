$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj') -- reproduce
exit $LASTEXITCODE
