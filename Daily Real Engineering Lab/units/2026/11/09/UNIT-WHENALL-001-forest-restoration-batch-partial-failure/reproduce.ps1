$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/run.ps1"
if ($LASTEXITCODE -ne 1) { throw "Expected original starter to fail contract with exit 1; got $LASTEXITCODE" }
Write-Host "REPRODUCED: starter violates batch outcome contract"
