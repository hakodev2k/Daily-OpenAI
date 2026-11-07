param()
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed. The learner-editable starter path still permits the stale write or regressed expected behavior.'
}

Write-Host 'Verification passed.'
