$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CohortLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --no-restore
exit $LASTEXITCODE
