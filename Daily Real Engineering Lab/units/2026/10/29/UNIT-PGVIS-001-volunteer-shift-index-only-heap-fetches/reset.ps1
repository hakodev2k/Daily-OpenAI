$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/scripts/common.ps1"
Assert-LabContainer
Invoke-LabSqlFile 'starter/schema.sql'
Write-Host 'Fixture reset (dedicated lab table only)'
