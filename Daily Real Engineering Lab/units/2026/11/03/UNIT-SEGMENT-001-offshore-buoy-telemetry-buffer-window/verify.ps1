$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj') -- verify
exit $LASTEXITCODE
