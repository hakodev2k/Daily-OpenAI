param([int]$Port = 5077)
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/scripts/Invoke-Lab.ps1" -Mode reproduce -Port $Port
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
