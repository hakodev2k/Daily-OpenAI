$ErrorActionPreference = 'Stop'
node ./starter/search.js verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter can still render an obsolete query result.' }
Write-Host 'VERIFY_PASS: final rendered result matches the latest user intent.'