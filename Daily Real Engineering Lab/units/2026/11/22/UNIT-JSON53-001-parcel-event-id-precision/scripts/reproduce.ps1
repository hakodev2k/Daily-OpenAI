$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/ParcelEvents.csproj')
if ($LASTEXITCODE -eq 1) { Write-Host 'REPRODUCED: starter rejected a distinct event.'; exit 0 }
Write-Error 'Expected starter failure (exit 1) was not observed.'
exit 1
