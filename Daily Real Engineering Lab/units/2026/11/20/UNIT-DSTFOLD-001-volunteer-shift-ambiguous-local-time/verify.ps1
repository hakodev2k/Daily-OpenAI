$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/ShiftClock.csproj') -- --check
if($LASTEXITCODE -ne 0){throw 'Learner workspace verification failed'}
Write-Host 'VERIFIED: all six behavior checks passed'
