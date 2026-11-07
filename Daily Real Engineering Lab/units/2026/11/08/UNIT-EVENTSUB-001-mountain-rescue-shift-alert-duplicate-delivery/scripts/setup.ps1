$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RescueAlerts.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
