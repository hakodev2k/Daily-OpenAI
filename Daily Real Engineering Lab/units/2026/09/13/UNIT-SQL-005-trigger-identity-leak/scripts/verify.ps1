$ErrorActionPreference = 'Stop'

$server = if ($env:LAB_SQL_SERVER) { $env:LAB_SQL_SERVER } else { 'localhost' }
$user = if ($env:LAB_SQL_USER) { $env:LAB_SQL_USER } else { 'sa' }
$password = if ($env:LAB_SQL_PASSWORD) { $env:LAB_SQL_PASSWORD } else { 'Your_strong_Password123!' }
$schema = Join-Path $PSScriptRoot 'schema.sql'
$starter = Join-Path (Split-Path $PSScriptRoot -Parent) 'starter/create-receipt.sql'

sqlcmd -S $server -U $user -P $password -C -b -i $schema
if ($LASTEXITCODE -ne 0) { throw 'Database reset failed.' }

sqlcmd -S $server -U $user -P $password -C -b -d EngineeringLabSql005 -i $starter
if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed. The learner-editable starter still returns the wrong receipt identity.'
}

Write-Host 'Verification passed: returned ID matches the inserted receipt while the audit trigger remains active.'
exit 0
