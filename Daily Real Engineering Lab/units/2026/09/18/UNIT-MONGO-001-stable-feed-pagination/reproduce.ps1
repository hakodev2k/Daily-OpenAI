$ErrorActionPreference='Continue'
& "$PSScriptRoot/run.ps1"
if ($LASTEXITCODE -eq 2) { Write-Host 'EXPECTED_FAILURE_REPRODUCED'; exit 0 }
Write-Error 'Expected pagination failure was not reproduced.'
exit 1