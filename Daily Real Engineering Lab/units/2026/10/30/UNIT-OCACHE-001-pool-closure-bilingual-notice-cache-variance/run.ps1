$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PoolNotices.csproj'
dotnet run --project $project --no-launch-profile -- --urls 'http://127.0.0.1:5193'
exit $LASTEXITCODE
