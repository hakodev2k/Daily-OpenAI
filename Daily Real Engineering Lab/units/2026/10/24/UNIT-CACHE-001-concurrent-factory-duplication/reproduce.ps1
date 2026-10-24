$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'baseline/CacheFactoryLab.Baseline.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
