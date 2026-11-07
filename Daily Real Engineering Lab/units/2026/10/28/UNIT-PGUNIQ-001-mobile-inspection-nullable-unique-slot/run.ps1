$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/reproduce.ps1"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
