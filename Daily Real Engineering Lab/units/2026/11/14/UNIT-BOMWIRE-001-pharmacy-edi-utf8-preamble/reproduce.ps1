$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PharmacyEdi.csproj'
dotnet run --project $project -- export
if ($LASTEXITCODE -ne 0) { throw 'Exporter failed' }
dotnet run --project $project -- inspect
if ($LASTEXITCODE -eq 0) { throw 'Expected gateway rejection, but inspection passed' }
Write-Host 'REPRODUCED: simulated gateway rejected manifest'
