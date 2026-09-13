$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/MetricUnitLab.csproj'

dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner project build failed.' }

dotnet run --project $project --no-build
if ($LASTEXITCODE -ne 0) {
    throw 'Verification FAIL: learner-editable starter still violates the telemetry contract.'
}

Write-Host 'Verification PASS: telemetry contract is consistent and functional behavior is preserved.'
