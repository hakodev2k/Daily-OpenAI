$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/ParcelEvents.csproj')
if ($LASTEXITCODE -ne 0) { Write-Error 'Verification failed: starter still loses an event.'; exit 1 }
Write-Host 'PASS: all three distinct events accepted.'
