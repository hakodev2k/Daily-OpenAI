$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/BikeSettlement.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { Write-Error 'FAIL: learner code mismatches contract'; exit 1 }
Write-Host 'PASS: learner code verified'; exit 0
