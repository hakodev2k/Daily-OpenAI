$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/PassLab.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Baseline execution failed unexpectedly' }
Write-Host 'Observe issued vs cache: expected baseline issued=2, cache=1. If timing differs, rerun.'
