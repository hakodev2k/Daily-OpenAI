$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/WaterAlerts.csproj') -- --reproduce
exit $LASTEXITCODE
