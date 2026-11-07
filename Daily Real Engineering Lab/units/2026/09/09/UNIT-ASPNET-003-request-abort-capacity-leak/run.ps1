$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RequestAbortCapacityLeak.csproj'
dotnet run --project $project -- run
exit $LASTEXITCODE
