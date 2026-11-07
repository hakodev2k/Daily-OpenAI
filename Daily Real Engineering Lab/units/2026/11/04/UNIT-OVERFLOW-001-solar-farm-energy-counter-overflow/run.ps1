$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SolarLedger.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
