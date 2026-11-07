$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/BikeSettlement.csproj') -- --verify
if ($LASTEXITCODE -eq 1) { Write-Host 'PASS: original mismatch reproduced'; exit 0 }
Write-Error 'Expected original mismatch was not reproduced'; exit 1
