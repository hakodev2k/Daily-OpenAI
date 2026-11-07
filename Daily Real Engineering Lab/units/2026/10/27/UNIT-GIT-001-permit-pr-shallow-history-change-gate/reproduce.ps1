$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1') | Out-Null
$work = Join-Path $PSScriptRoot '.lab-runtime/shallow'
$baseSha = (Get-Content (Join-Path $PSScriptRoot '.lab-runtime/base.sha') -Raw).Trim()
$shallowState = (& git -C $work rev-parse --is-shallow-repository).Trim()
$output = @(& (Join-Path $PSScriptRoot 'baseline/detect-changes.ps1') -WorkingCopy $work -BaseSha $baseSha)
$code = $LASTEXITCODE
if ($shallowState -eq 'true' -and $code -eq 12 -and ($output -join ' ') -match 'CHANGE_GATE_ERROR') {
    Write-Output 'REPRODUCED: CI change gate fails on the local checkout.'
    Write-Output 'Inspect .lab-runtime/shallow, .lab-runtime/base.sha and evidence/ci-output.md.'
    exit 0
}
Write-Output "REPRODUCE FAILED: shallow=$shallowState, exit=$code, output=$($output -join ' ')"
exit 1
