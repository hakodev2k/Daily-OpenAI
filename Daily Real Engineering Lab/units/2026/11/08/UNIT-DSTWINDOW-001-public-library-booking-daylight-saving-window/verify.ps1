$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/BookingLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Regression verification failed for learner-edited starter' }
Write-Host 'Verification passed for learner-edited starter.'
