$ErrorActionPreference = 'Stop'
dotnet --version
dotnet restore (Join-Path $PSScriptRoot 'starter/OccupancyGateLab.csproj')
if ($LASTEXITCODE -ne 0) { throw "Restore failed: $LASTEXITCODE" }
