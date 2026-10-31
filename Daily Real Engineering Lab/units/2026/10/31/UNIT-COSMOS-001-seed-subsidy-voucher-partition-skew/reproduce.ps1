param()
$ErrorActionPreference = 'Stop'
$output = & pwsh -NoProfile -File (Join-Path $PSScriptRoot 'scripts/simulate.ps1') -PolicyPath (Join-Path $PSScriptRoot 'starter/policy.json') -Check 2>&1
$code = $LASTEXITCODE
$output | Out-Host
if ($code -eq 1 -and (($output | Out-String) -match 'FAIL: hotspot share')) {
    Write-Host 'REPRODUCED: starter violates the hotspot acceptance check.'
    exit 0
}
Write-Host ('NOT REPRODUCED: unexpected simulator result (exit=' + $code + ').')
exit 1
