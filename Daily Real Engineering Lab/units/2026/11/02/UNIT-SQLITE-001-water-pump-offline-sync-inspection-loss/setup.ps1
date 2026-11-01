$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PumpSyncLab.csproj'
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Dependencies restored. Requires .NET 8 SDK.'
