$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/TreeInventory.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: conditional update contract chưa đạt.' }
Write-Host 'Learner code passed the deterministic replay.'
