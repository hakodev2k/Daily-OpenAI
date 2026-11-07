$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnergyLedger.csproj'
& dotnet run --project $project -- demo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
