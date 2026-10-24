param([int]$Port = 5077, [switch]$Reference)
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/scripts/Invoke-Lab.ps1" -Mode verify -Port $Port -Reference:$Reference
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
