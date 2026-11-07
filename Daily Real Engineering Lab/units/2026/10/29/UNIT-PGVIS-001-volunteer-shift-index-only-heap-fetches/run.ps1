$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/scripts/common.ps1"
Assert-LabContainer
$p=Get-LabPlan
$n=Get-LabCount
Write-Host "Node=$($p.NodeType); Heap Fetches=$($p.HeapFetches); Actual Rows=$($p.ActualRows); Count=$n"
