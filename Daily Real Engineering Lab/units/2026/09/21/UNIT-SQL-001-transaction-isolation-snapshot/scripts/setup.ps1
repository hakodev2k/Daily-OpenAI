$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
sqlcmd -S localhost -E -b -i "$root/starter/setup.sql"
if ($LASTEXITCODE -ne 0) { throw 'Setup failed.' }
Write-Host 'Lab database ready.'