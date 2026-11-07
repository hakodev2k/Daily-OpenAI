param()
$ErrorActionPreference = 'Stop'
& pwsh -NoProfile -File (Join-Path $PSScriptRoot 'scripts/simulate.ps1') -PolicyPath (Join-Path $PSScriptRoot 'starter/policy.json')
exit $LASTEXITCODE
