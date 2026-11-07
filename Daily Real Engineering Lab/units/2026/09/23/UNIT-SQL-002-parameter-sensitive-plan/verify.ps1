param([string]$Server='localhost')
$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host 'Running learner-editable query path...'
sqlcmd -S $Server -E -d EngineeringLabSql002 -i (Join-Path $root 'starter/query.sql')
if ($LASTEXITCODE -ne 0) { throw 'Learner query failed.' }
$counts = sqlcmd -S $Server -E -d EngineeringLabSql002 -h -1 -W -Q "SET NOCOUNT ON; SELECT CONCAT((SELECT COUNT(*) FROM dbo.Orders WHERE TenantId=1 AND CreatedAt>='2026-01-01'),':',(SELECT COUNT(*) FROM dbo.Orders WHERE TenantId=999 AND CreatedAt>='2026-01-01'));"
if (-not ($counts -match '^\d+:\d+$')) { throw 'Correctness check could not read expected tenant counts.' }
Write-Host "Correctness baseline counts: $counts"
Write-Host 'Verification requires you to compare logical reads and Actual-vs-Estimated rows for both tenant shapes. Timing alone is not a pass criterion.'