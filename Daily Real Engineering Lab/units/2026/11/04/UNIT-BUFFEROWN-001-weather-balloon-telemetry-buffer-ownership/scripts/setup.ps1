$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet restore (Join-Path $unit 'starter/TelemetryLab.csproj')
exit $LASTEXITCODE
