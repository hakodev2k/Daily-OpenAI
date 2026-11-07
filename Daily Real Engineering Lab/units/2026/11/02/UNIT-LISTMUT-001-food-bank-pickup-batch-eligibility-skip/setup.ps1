$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'checks/Checks.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
