$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CultureIdentifierLab.csproj'

dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) {
    throw "Verification failed. The learner-editable starter still rejects the identifier."
}

Write-Host 'Verification passed: identifier matching is stable for the simulated culture.'
