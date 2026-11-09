$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/run.ps1"
if ($LASTEXITCODE -ne 0) { throw "Verification failed: learner-editable starter still violates contract" }
Write-Host "VERIFIED: all input sites accounted for"
