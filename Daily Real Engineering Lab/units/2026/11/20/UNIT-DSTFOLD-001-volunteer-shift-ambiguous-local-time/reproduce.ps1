$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/ShiftClock.csproj') -- --check
if($LASTEXITCODE -eq 0){throw 'Starter unexpectedly passed'}
Write-Host 'REPRODUCED: starter fails expected contract assertions'
