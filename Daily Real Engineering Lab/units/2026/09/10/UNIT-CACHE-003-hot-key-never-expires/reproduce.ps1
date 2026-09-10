$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CacheExpirationLab.csproj'
dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
