$ErrorActionPreference = 'Stop'
node ./starter/search.js reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter no longer reproduces the intended stale-result symptom.' }
Write-Host 'REPRODUCED: final rendered query does not match the latest input.'