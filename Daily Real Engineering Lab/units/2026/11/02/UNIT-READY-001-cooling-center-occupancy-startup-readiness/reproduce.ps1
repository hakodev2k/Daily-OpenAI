$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/OccupancyGateLab.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Starter symptom not reproduced: $LASTEXITCODE" }
