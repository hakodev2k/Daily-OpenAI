$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/scripts/common.ps1"
Assert-LabContainer
Invoke-LabSqlFile 'starter/schema.sql'
Invoke-LabSqlFile 'starter/churn.sql'
$before=Get-LabPlan
if ($before.HeapFetches -le 0) { throw 'Cannot verify: original symptom absent' }
Invoke-LabSqlFile 'workspace/remediation.sql'
$after=Get-LabPlan
$count=Get-LabCount
Write-Host "Before=$($before.HeapFetches); after=$($after.HeapFetches); count=$count"
if ($count -ne 5000 -or $after.ActualRows -ne 5000) { throw 'Functional regression: expected 5000 rows' }
if ($after.HeapFetches -ne 0) { throw "NOT FIXED: Heap Fetches=$($after.HeapFetches)" }
Write-Host 'VERIFIED: learner remediation passed'
