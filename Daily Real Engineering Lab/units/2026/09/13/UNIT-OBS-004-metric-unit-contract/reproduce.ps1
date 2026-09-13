$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/MetricUnitLab.csproj'

dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Starter build failed.' }

dotnet run --project $project --no-build
$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    throw 'Expected the original starter telemetry contract to fail, but it passed.'
}

Write-Host 'Reproduction PASS: starter exposed the intended telemetry contract failure.'
exit 0
