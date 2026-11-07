$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project 'tests/MetricContract.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed verification.' }
    Write-Host 'VERIFIED: learner-editable starter satisfies metric contract.'
} finally { Pop-Location }
