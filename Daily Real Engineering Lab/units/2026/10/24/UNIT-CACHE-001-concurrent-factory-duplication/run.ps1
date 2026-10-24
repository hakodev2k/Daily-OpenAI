$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/CacheFactoryLab.csproj') -- run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
