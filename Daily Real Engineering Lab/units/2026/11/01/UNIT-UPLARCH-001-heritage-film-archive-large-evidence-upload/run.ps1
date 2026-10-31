$ErrorActionPreference = 'Stop'
$result = & (Join-Path $PSScriptRoot 'scripts/evaluate.ps1') -PlanPath (Join-Path $PSScriptRoot 'starter/architecture.json') -ConstraintsPath (Join-Path $PSScriptRoot 'starter/constraints.json')
$result | Format-List
