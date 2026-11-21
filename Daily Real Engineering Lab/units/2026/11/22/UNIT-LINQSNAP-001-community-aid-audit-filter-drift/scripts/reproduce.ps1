$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/AuditSnapshotLab.csproj') -- --verify
if ($LASTEXITCODE -eq 0) { Write-Error 'Expected starter discrepancy not observed (or starter already fixed).'; exit 1 }
Write-Host 'Expected failure reproduced.'
exit 0
