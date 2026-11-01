$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnergyLedger.csproj'
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'RESTORE PASS'
