$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RequestBodyAuditLab.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Learner-editable starter passed behavioral contracts.'
