$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RequestBodyAuditLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Restore OK.'
