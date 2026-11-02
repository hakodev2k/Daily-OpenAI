$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/OccupancyGateLab.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw "Learner verification failed: $LASTEXITCODE" }
