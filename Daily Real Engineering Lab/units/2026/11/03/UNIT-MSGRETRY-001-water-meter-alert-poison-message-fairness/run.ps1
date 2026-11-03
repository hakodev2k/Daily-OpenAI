$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/WaterAlerts.csproj') -- --demo
exit $LASTEXITCODE
