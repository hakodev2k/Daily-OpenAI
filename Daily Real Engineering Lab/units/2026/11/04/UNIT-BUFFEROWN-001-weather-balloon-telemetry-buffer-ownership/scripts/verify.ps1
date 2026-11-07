$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/TelemetryLab.csproj') -- verify
exit $LASTEXITCODE
