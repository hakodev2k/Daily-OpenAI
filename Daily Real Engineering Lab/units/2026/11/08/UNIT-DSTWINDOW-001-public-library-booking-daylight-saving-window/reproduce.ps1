$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/BookingLab.csproj') -- --inspect
if ($LASTEXITCODE -ne 0) { throw 'Starter inspection failed to run' }
Write-Host 'Reproduction: inspect FAIL rows (expected before learner fix).'
