$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'OccupancyGateLab.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw "Reference self-check failed: $LASTEXITCODE" }
