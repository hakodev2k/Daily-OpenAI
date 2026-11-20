$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/ShelterIntake.csproj') -- --check
if ($LASTEXITCODE -ne 0) { throw 'Invariant failed.' }
