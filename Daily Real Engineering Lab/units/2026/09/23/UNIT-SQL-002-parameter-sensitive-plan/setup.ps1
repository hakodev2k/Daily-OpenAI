param([string]$Server='localhost')
$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
sqlcmd -S $Server -E -i (Join-Path $root 'starter/setup.sql')
if ($LASTEXITCODE -ne 0) { throw 'Lab setup failed.' }
Write-Host 'EngineeringLabSql002 is ready.'