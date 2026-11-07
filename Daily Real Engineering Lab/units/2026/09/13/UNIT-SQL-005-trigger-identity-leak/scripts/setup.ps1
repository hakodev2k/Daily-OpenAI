$ErrorActionPreference = 'Stop'

$server = if ($env:LAB_SQL_SERVER) { $env:LAB_SQL_SERVER } else { 'localhost' }
$user = if ($env:LAB_SQL_USER) { $env:LAB_SQL_USER } else { 'sa' }
$password = if ($env:LAB_SQL_PASSWORD) { $env:LAB_SQL_PASSWORD } else { 'Your_strong_Password123!' }
$schema = Join-Path $PSScriptRoot 'schema.sql'

Write-Host "Preparing lab database on $server ..."
sqlcmd -S $server -U $user -P $password -C -b -i $schema
if ($LASTEXITCODE -ne 0) { throw 'Database setup failed.' }

Write-Host 'Database ready.'
Write-Host 'Set LAB_SQL_SERVER / LAB_SQL_USER / LAB_SQL_PASSWORD if your local SQL Server uses different values.'
