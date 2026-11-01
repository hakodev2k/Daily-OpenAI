$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RosterLab.csproj'
& dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet restore $project
exit $LASTEXITCODE
