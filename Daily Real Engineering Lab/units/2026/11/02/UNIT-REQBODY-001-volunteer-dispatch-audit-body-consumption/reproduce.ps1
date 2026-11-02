$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RequestBodyAuditLab.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Original symptom reproduced. This is NOT a fix verification.'
