$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/scripts/common.ps1"
& "$PSScriptRoot/setup.ps1"
if ($LASTEXITCODE -ne 0) { throw 'Setup failed' }
Assert-LabContainer
Invoke-LabSqlFile 'starter/schema.sql'
$baseline=Get-LabPlan
Invoke-LabSqlFile 'starter/churn.sql'
$after=Get-LabPlan
$count=Get-LabCount
Write-Host "Baseline heap fetches=$($baseline.HeapFetches); after sync=$($after.HeapFetches); rows=$count"
if ($count -ne 5000 -or $baseline.HeapFetches -ne 0 -or $after.HeapFetches -le 0) { throw 'Fixture did not reproduce the intended symptom' }
Write-Host 'REPRODUCED'
