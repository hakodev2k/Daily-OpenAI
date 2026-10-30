$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'scripts/measure.ps1') -PolicyPath (Join-Path $PSScriptRoot 'starter/metric-policy.json')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
