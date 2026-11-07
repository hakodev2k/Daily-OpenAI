$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/WaterAlerts.csproj') -- --verify
exit $LASTEXITCODE
