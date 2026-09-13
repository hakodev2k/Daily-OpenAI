$ErrorActionPreference = 'Stop'

$server = if ($env:LAB_SQL_SERVER) { $env:LAB_SQL_SERVER } else { 'localhost' }
$user = if ($env:LAB_SQL_USER) { $env:LAB_SQL_USER } else { 'sa' }
$password = if ($env:LAB_SQL_PASSWORD) { $env:LAB_SQL_PASSWORD } else { 'Your_strong_Password123!' }
$schema = Join-Path $PSScriptRoot 'schema.sql'
$starter = Join-Path (Split-Path $PSScriptRoot -Parent) 'starter/create-receipt.sql'

sqlcmd -S $server -U $user -P $password -C -b -i $schema
if ($LASTEXITCODE -ne 0) { throw 'Database reset failed.' }

sqlcmd -S $server -U $user -P $password -C -b -d EngineeringLabSql005 -i $starter
if ($LASTEXITCODE -eq 0) {
    throw 'Expected the starter state to reproduce the wrong returned identity, but it passed.'
}

Write-Host 'Reproduction succeeded: the starter returned an identity that does not identify the inserted receipt.'
exit 0
