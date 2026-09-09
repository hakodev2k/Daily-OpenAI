$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RequestAbortCapacityLeak.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet run --project $project -- verify
exit $LASTEXITCODE
