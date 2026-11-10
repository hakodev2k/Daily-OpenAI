$ErrorActionPreference='Stop'
& (Join-Path $PSScriptRoot 'run.ps1')
if ($LASTEXITCODE -eq 2) { Write-Host 'EXPECTED REPRODUCTION'; exit 0 }
Write-Error "Expected exit 2, got $LASTEXITCODE"; exit 1
