$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CacheExpirationLab.csproj'
dotnet run --project $project -- run
