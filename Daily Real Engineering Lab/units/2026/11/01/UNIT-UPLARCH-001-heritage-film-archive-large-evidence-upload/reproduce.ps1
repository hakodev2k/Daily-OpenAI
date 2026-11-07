$ErrorActionPreference = 'Stop'
$result = & (Join-Path $PSScriptRoot 'scripts/evaluate.ps1') -PlanPath (Join-Path $PSScriptRoot 'starter/architecture.json') -ConstraintsPath (Join-Path $PSScriptRoot 'starter/constraints.json')
$result | Format-List
if ($result.Passed) { Write-Error 'REPRODUCTION FAILED: starter unexpectedly passes.'; exit 1 }
if ($result.EstimatedPeakMemoryMiBPerInstance -le 1536) { Write-Error 'REPRODUCTION FAILED: memory symptom missing.'; exit 1 }
Write-Host 'REPRODUCED: starter violates capacity and control requirements.'
exit 0
