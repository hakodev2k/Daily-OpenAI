$ErrorActionPreference = 'Stop'
$result = & (Join-Path $PSScriptRoot 'scripts/evaluate.ps1') -PlanPath (Join-Path $PSScriptRoot 'workspace/architecture.json') -ConstraintsPath (Join-Path $PSScriptRoot 'starter/constraints.json')
$result | Format-List
if (-not $result.Passed) {
  Write-Host 'VERIFY FAIL: revise workspace/architecture.json and rerun.'
  exit 1
}
Write-Host 'VERIFY PASS: modeled capacity, budget and declared controls pass. Manual architecture/security review remains required.'
exit 0
